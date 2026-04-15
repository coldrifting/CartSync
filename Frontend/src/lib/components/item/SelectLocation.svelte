<script lang="ts">
    import type Aisle from "$lib/models/Aisle.ts";
    import type ItemDetails from "$lib/models/ItemDetails.ts";
    import {FormGroup, Input} from "@sveltestrap/sveltestrap";
    import {patch, put} from "$lib/functions/requests.js";
    import BayType from "$lib/models/BayType.js";
    import type Store from "$lib/models/Store.ts";
    import {QueryClient} from "@tanstack/svelte-query";

    interface Props {
        stores: Store[];
        aisles: Aisle[];
        item: ItemDetails;
    }

    let {stores, aisles, item}: Props = $props();
    
    const queryClient = new QueryClient()
    
    let selectedStoreId = $derived(stores.find(s => s.isSelected)!.id);
    let aislesByStore = $derived.by(() => {
        return aisles.filter(a => a.storeId === selectedStoreId).sort((a, b) => a.name > b.name ? 1 : -1);
    });
    
    let aisleId = $derived.by(() => {
        return item.locations.find(a => a.storeId === selectedStoreId)?.aisleId ?? "";
    });
    
    let bay = $derived.by(() => {
        return item.locations.find(a => a.storeId === selectedStoreId)?.bay ?? BayType.Types[1];
    })
    
    async function onStoreChange() {
        await put(`/api/stores/${selectedStoreId}/select`, {});
        await queryClient.invalidateQueries({queryKey: ['stores']});
    }
    
    async function onAisleChange() {
        await patch(`/api/items/${item.id}/edit`, {"/Location": {aisleId: aisleId, bay: bay}});
        await queryClient.invalidateQueries({queryKey: ['items']});
    }
    
    async function onBayChange() {
        await patch(`/api/items/${item.id}/edit`, {"/Location": {aisleId: aisleId, bay: bay}});
        await queryClient.invalidateQueries({queryKey: ['items']});
    }
</script>
<FormGroup floating label="Store">
    <Input type="select"
           name="storeId"
           bind:value={selectedStoreId}
           onchange={onStoreChange}>
        {#each stores as store}
            <option value="{store.id}">{store.name}</option>
        {/each}
    </Input>
</FormGroup>

<FormGroup floating label="Aisle">
    <Input type="select"
           name="aisleId"
           bind:value={aisleId}
           onchange={onAisleChange}>
        <option value="" disabled selected hidden>Select an Aisle</option>
        {#each aislesByStore as aisle}
            <option value="{aisle.id}">{aisle.name}</option>
        {/each}
    </Input>
</FormGroup>

{#if aisleId !== ""}
    <FormGroup floating label="Bay">
        <Input type="select"
               name="bay"
               bind:value={bay}
               onchange={onBayChange}>
            {#each BayType.Types as type}
                <option value="{type}">{type}</option>
            {/each}
        </Input>
    </FormGroup>
{/if}