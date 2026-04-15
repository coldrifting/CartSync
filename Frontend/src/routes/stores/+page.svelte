<script lang="ts">
    import {browser} from "$app/environment";
    import {redirect} from "@sveltejs/kit";
    import {goto} from "$app/navigation";
    import {patch, post, del, put, get, mutate} from "$lib/functions/requests.js";
    import ModalAdd from "$lib/components/modal/generic/ModalAdd.svelte";
    import ModalRename from "$lib/components/modal/generic/ModalRename.svelte";
    import Header from "$lib/components/nav/Header.svelte";
    import ListItemLink from "$lib/components/lists/ListItemLink.svelte";
    import ListItemRadio from "$lib/components/lists/ListItemRadio.svelte";
    import {createQuery, useQueryClient} from "@tanstack/svelte-query";
    import type Store from "$lib/models/Store.ts";
    import LoadingPage from "$lib/components/LoadingPage.svelte";
    
    const client = useQueryClient()
    
    const queryStores = createQuery(() => ({
        queryKey: ['stores'],
        queryFn: () => get<Store[]>('/api/stores', fetch),
    }))
    
    let addDialog: ModalAdd
    let renameDialog: ModalRename
    
    async function logout() {
        if (browser) {
            await goto('/logout');
        }
        else {
            throw redirect(303, "/logout");
        }
    }
    
    const headerActions: HeaderAction[] = [
        {label: "Logout", icon: "fa-sign-out", color: 'danger', action: logout, hideFromDesktop: true},
        {label: "Add", icon: "fa-plus", color: 'primary', action: () => {addDialog.show()}}
    ];
    
    async function addAction(value: string) {
        await post("/api/stores/add", {name: value});
        await client.invalidateQueries({queryKey: ['stores']})
    }
    
    async function renameAction(id: string, value: string) {
        await patch(`/api/stores/${id}/edit`, {"/Name": value});
        await client.invalidateQueries({queryKey: ['stores']})
    }
    
    async function deleteAction(id: string) {
        await del(`/api/stores/${id}/delete`);
        await client.invalidateQueries({queryKey: ['stores']})
    }
    
    const selectedStoreMutate = mutate<string, Store[]>(
        client, 
        ['stores'],
        (id) => put(`/api/stores/${id}/select`, {}),
        (query, id) => {
            const clone = structuredClone(query);
            let prevIndex = clone.map(s => s.isSelected).indexOf(true);
            let newIndex = clone.map(s => s.id).indexOf(id);
            if (prevIndex !== -1 && newIndex !== -1) {
                clone[prevIndex].isSelected = false;
                clone[newIndex].isSelected = true;
            }
            return clone;
        }
    );
    
    async function onChange(value: string) {
        selectedStoreMutate.mutate(value);
    }
</script>

<svelte:head>
    <title>CartSync - Stores</title>
</svelte:head>

<ModalAdd bind:this={addDialog} type="Store" addAction={addAction}/>
<ModalRename bind:this={renameDialog} type="Store" renameAction={renameAction} deleteAction={deleteAction} warning="All item locations for [Name] will be deleted!"/>

<Header title="Stores" headerActions={headerActions} />

{#if queryStores.isLoading}
    <LoadingPage/>
    {:else if queryStores.isError}
    <p>Error: {queryStores.error?.message}</p>
    {:else}
    {@const selectedStore = queryStores.data?.find(s => s.isSelected)}
<h4>Selected Store</h4>
<div>
    <ul>
        <ListItemLink label={selectedStore?.name ?? ""} 
                         href="/stores/{selectedStore?.id ?? ''}" 
                         info="Aisles" 
                         showArrow={true}/>
    </ul>
</div>

<h4>All Stores</h4>
<ul>
    {#each queryStores.data as store}
        <ListItemRadio
                id={store.id}
                label={store.name}
                group="selectedStoreId"
                selectedValue={selectedStore?.id ?? ''}
                actionRight={{
                    label: 'Edit', 
                    icon: 'fa-pencil', 
                    color: 'success', 
                    action: () => renameDialog.show(store.id, store.name, !store.isSelected)
                }}
                onValueChange={onChange}
        />
    {/each}
</ul>
    {/if}
