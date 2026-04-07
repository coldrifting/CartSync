<script lang="ts">
    import {enhance} from '$app/forms';
    import type {PageProps} from './$types';
    import {FormGroup, Input} from "@sveltestrap/sveltestrap";
    import type ItemDetails from "$lib/scripts/classes/ItemDetails.ts";
    import ItemTemp from "$lib/scripts/classes/ItemTemp.js";
    import BayType from "$lib/scripts/classes/BayType.js";
    import UnitType from "$lib/scripts/classes/UnitType.js";
    import Header from "$lib/components/Header.svelte";
    import ListElementLink from "$lib/components/ListElementLink.svelte";

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

    let itemTempForm: HTMLFormElement;
    let itemDefaultUnitsForm: HTMLFormElement;
    let storeForm: HTMLFormElement;
    let itemAisleForm: HTMLFormElement;
    let itemAisleBayForm: HTMLFormElement;

    let stores = $derived(data.stores);
    let selectedStoreId = $derived(data.selectedStore.id);
    let aisles = $derived(data.aisles.sort((a, b) => a.name > b.name ? 1 : -1));
    let aisleId = $derived(item.location?.aisleId ?? "");
    let bay = $derived(item.location?.bay ?? BayType.Types[1]);
</script>

<svelte:head>
    <title>{data.item.name}</title>
</svelte:head>

<Header back={['/items', 'Items']} title={item.name}/>

<h4>Details</h4>
<form method="POST"
      action="?/editItemTemp"
      bind:this={itemTempForm}
      use:enhance>
    <input name="itemId" bind:value={itemId} hidden/>
    <FormGroup floating label="Temperature">
        <Input type="select"
               name="itemTemp"
               bind:value={itemTemp}
               onchange={() => itemTempForm.requestSubmit()}>
            {#each ItemTemp.Temps as temp}
                <option value="{temp}">{temp}</option>
            {/each}
        </Input>
    </FormGroup>
</form>
<form method="POST"
      action="?/editItemDefaultUnits"
      bind:this={itemDefaultUnitsForm}
      use:enhance>
    <input name="itemId" bind:value={itemId} hidden/>
    <FormGroup floating label="Default Units">
        <Input type="select"
               name="itemDefaultUnits"
               bind:value={itemDefaultUnits}
               onchange={() => itemDefaultUnitsForm.requestSubmit()}>
            {#each UnitType.Types as type}
                <option value="{type}">{UnitType.asString(type)}</option>
            {/each}
        </Input>
    </FormGroup>
</form>

<h4>Preps</h4>

<ListElementLink id="Preps"
                 label={prepText === "" ? "(None)" : prepText}
                 link="/items/{itemId}/preps"
                 info="Edit"/>

<h4>Location</h4>
<form method="POST"
      action="?/editCurrentStore"
      bind:this={storeForm}
      use:enhance>
    <FormGroup floating label="Store">
        <Input type="select"
               name="storeId"
               bind:value={selectedStoreId}
               onchange={() => storeForm.requestSubmit()}>
            {#each stores as store}
                <option value="{store.id}">{store.name}</option>
            {/each}
        </Input>
    </FormGroup>
</form>

<form method="POST"
      action="?/editItemAisle"
      bind:this={itemAisleForm}
      use:enhance>
    <FormGroup floating label="Aisle">
        <input name="itemId" bind:value={itemId} hidden/>
        <input name="bay" bind:value={bay} hidden/>
        <Input type="select"
               name="aisleId"
               bind:value={aisleId}
               placeholder="Blah"
               onchange={() => itemAisleForm.requestSubmit()}>
            <option value="" disabled selected hidden>Select an Aisle</option>
            {#each aisles as aisle}
                <option value="{aisle.id}">{aisle.name}</option>
            {/each}
        </Input>
    </FormGroup>
</form>

<form method="POST"
      action="?/editItemAisle"
      bind:this={itemAisleBayForm}
      use:enhance>
    {#if aisleId !== ""}
        <input name="itemId" bind:value={itemId} hidden/>
        <input name="aisleId" bind:value={aisleId} hidden/>
        <FormGroup floating label="Bay">
            <Input type="select"
                   name="bay"
                   bind:value={bay}
                   onchange={() => itemAisleBayForm.requestSubmit()}>
                {#each BayType.Types as type}
                    <option value="{type}">{type}</option>
                {/each}
            </Input>
        </FormGroup>
    {/if}
</form>