<script lang="ts">
    import {enhance} from '$app/forms';
    import type {PageProps} from './$types';
    import ModalAdd from "$lib/components/modal/generic/ModalAdd.svelte";
    import ModalRename from "$lib/components/modal/generic/ModalRename.svelte";
    import Header from "$lib/components/nav/Header.svelte";
    import ListItemCheckbox from "$lib/components/lists/ListItemCheckbox.svelte";
    import {get, post, patch, del} from "$lib/functions/requests.js";
    import PrepUsagesReport from "$lib/models/PrepUsagesReport.js";
    import {invalidateAll} from "$app/navigation";

    let {data}: PageProps = $props();
    let itemId: string = $derived(data.item.id);
    
    let preps = $derived(data.preps);
    
    let addDialog: ModalAdd
    let renameDialog: ModalRename
    
    const headerActions: HeaderAction[] = [
        {label: "Add Prep", icon: "fa-plus", action: () => {addDialog.show()}}
    ];
    
    async function onAdd(value: string) {
        await post('/api/preps/add', {name: value});
    }
    
    async function onRename(id: string, value: string) {
        await patch(`/api/preps/${id}/edit`, {"/Name": value});
    }
    
    async function onDelete(id: string) {
        await del(`/api/preps/${id}/delete`);
    }
    
    async function onTryDelete(id: string): Promise<Record<string, string[]>> {
        const usages = await get<PrepUsagesReport>(`/api/preps/${id}/usages`);
        return PrepUsagesReport.getUsages(usages);
    }
    
    async function onPrepsChanged(id: string, value: boolean) {
        const prepIds = data.item.preps.map(itemPrep => itemPrep.id);
        if (value) {
            prepIds.push(id);
        }
        else {
            const index = prepIds.indexOf(id);
            if (index >= 0) {
                prepIds.splice(index, 1);
            }
        }
        await patch(`/api/items/${data.item.id}/edit`, {"/PrepIds": prepIds});
        await invalidateAll();
    }
</script>

<svelte:head>
    <title>{data.item.name} - Preps</title>
</svelte:head>

<ModalAdd bind:this={addDialog} type="Prep" addAction={onAdd}/>
<ModalRename bind:this={renameDialog} type="Prep" renameAction={onRename} deleteAction={onDelete} tryDeleteAction={onTryDelete}/>

<Header back={[`/items/${itemId}`, 'Item']} 
        title={data.item.name} 
        subtitle="Preps" 
        headerActions={headerActions} />

<form method="POST"
      action="?/editPreps"
      id="editPrepForm"
      use:enhance>
    <input name="itemId" bind:value={itemId} hidden/>
    <ul>
        {#each preps as prep}
            <ListItemCheckbox
                    id={prep.prepId}
                    label={prep.prepName}
                    name="selectedPrepIds"
                    checked={prep.isSelected}
                         actionRight={{
                            label: 'Edit', 
                            icon: 'fa-pencil', 
                            color: 'success', 
                            action: () => renameDialog.show(prep.prepId, prep.prepName, true)
                         }}
                    onValueChange={onPrepsChanged}
            />
        {/each}
    </ul>
</form>