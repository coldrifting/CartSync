<script lang="ts">
    import type {PageProps} from './$types';
    import {FormGroup, Input} from "@sveltestrap/sveltestrap";
    import type ItemDetails from "$lib/models/ItemDetails.ts";
    import ItemTemp from "$lib/models/ItemTemp.js";
    import BayType from "$lib/models/BayType.js";
    import UnitType from "$lib/models/UnitType.js";
    import Header from "$lib/components/nav/Header.svelte";
    import ListItemLink from "$lib/components/lists/ListItemLink.svelte";
    import {patch, put} from "$lib/functions/requests.js";
    import {invalidateAll} from "$app/navigation";

    let {data}: PageProps = $props();
    let item: ItemDetails = $derived(data.item);

    let itemId: string = $derived(data.item.id);
    let itemTemp: string = $derived(data.item.temp);
    let itemDefaultUnits: string = $derived(data.item.defaultUnitType);

    let prepText = $derived(data.item.preps
            .map(prep => prep.name)
            .slice(0, 3)
            .join(", ") +
        (data.item.preps.length > 3 ? ", ..." : ""));

    let stores = $derived(data.stores);
    let selectedStoreId = $derived(data.selectedStore.id);
    let aisles = $derived(data.aisles.sort((a, b) => a.name > b.name ? 1 : -1));
    let aisleId = $derived(item.location?.aisleId ?? "");
    let bay = $derived(item.location?.bay ?? BayType.Types[1]);
    
    async function onTempChange() {
        await patch(`/api/items/${itemId}/edit`, {"/Temp": itemTemp});
        await invalidateAll();
    }
    
    async function onDefaultUnitsChange() {
        await patch(`/api/items/${itemId}/edit`, {"/DefaultUnitType": itemDefaultUnits});
        await invalidateAll();
    }
    
    async function onStoreChange() {
        await put(`/api/stores/${selectedStoreId}/select`, {});
        await invalidateAll();
    }
    
    async function onLocationChange() {
        await patch(`/api/items/${itemId}/edit`, {"/Location": {aisleId: aisleId, bay: bay}});
        await invalidateAll();
    }
</script>

<svelte:head>
    <title>{data.item.name}</title>
</svelte:head>

<Header back={['/items', 'Items']} title={item.name}/>

<h4>Details</h4>
<FormGroup floating label="Temperature">
    <Input type="select"
           name="itemTemp"
           bind:value={itemTemp}
           onchange={() => onTempChange()}>
        {#each ItemTemp.Temps as temp}
            <option value="{temp}">{temp}</option>
        {/each}
    </Input>
</FormGroup>
<FormGroup floating label="Default Units">
    <Input type="select"
           name="itemDefaultUnits"
           bind:value={itemDefaultUnits}
           onchange={() => onDefaultUnitsChange()}>
        {#each UnitType.Types as type}
            <option value="{type}">{UnitType.asString(type)}</option>
        {/each}
    </Input>
</FormGroup>

<h4>Preps</h4>

<ListItemLink label={prepText === "" ? "(None)" : prepText}
                 showArrow={true}
                 href="/items/{itemId}/preps"
                 info="Edit"/>

<h4>Location</h4>
<FormGroup floating label="Store">
    <Input type="select"
           name="storeId"
           bind:value={selectedStoreId}
           onchange={() => onStoreChange()}>
        {#each stores as store}
            <option value="{store.id}">{store.name}</option>
        {/each}
    </Input>
</FormGroup>

<FormGroup floating label="Aisle">
    <input name="itemId" bind:value={itemId} hidden/>
    <input name="bay" bind:value={bay} hidden/>
    <Input type="select"
           name="aisleId"
           bind:value={aisleId}
           placeholder="Blah"
           onchange={() => onLocationChange()}>
        <option value="" disabled selected hidden>Select an Aisle</option>
        {#each aisles as aisle}
            <option value="{aisle.id}">{aisle.name}</option>
        {/each}
    </Input>
</FormGroup>

{#if aisleId !== ""}
    <input name="itemId" bind:value={itemId} hidden/>
    <input name="aisleId" bind:value={aisleId} hidden/>
    <FormGroup floating label="Bay">
        <Input type="select"
               name="bay"
               bind:value={bay}
               onchange={() => onLocationChange()}>
            {#each BayType.Types as type}
                <option value="{type}">{type}</option>
            {/each}
        </Input>
    </FormGroup>
{/if}