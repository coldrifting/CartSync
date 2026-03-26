<script lang="ts">
    import MultiSelect, {type Option} from 'svelte-multiselect'
    import type {PageProps} from './$types';
    import ItemTemp from '$lib/types/ItemTemp.js'
    import UnitType from '$lib/types/UnitType.js'
    import BayType from "$lib/types/BayType.js";
    import LinkHeader from "$lib/components/LinkHeader.svelte";
    import {Button, FormGroup, Input} from "@sveltestrap/sveltestrap";
	import { enhance } from '$app/forms';

    let {data}: PageProps = $props();
    let ingredient: Ingredient = $derived(data.ingredient);
    
    let itemId: string = $derived(data.ingredient.itemId);
    let itemTemp: string = $derived(data.ingredient.itemTemp);
    let itemDefaultUnits: string = $derived(data.ingredient.defaultUnitType);
    
    let allPreps = $derived(data.allPreps);
    let selectedPreps = $derived(data.selectedPreps);
    
	let itemTempForm: HTMLFormElement;
    let itemDefaultUnitsForm: HTMLFormElement;
    
</script>

<LinkHeader url="/ingredients" title="Ingredients"/>
<h2 class="text-center">{ingredient.itemName}</h2>
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
                <option value="{type}">{UnitType.ToDisplay(type)}</option>
            {/each}
        </Input>
    </FormGroup>
</form>

<h4>Preps</h4>
<!-- TODO Fix state proxy waring -->
<MultiSelect bind:selected={selectedPreps} options={allPreps} />

<Button class="mt-1 text-end btn-link bg-transparent">Add New Prep</Button>

<!-- TODO Add Location logic-->
<h4 class="pt-4">Location</h4>
{#if ingredient.locations.length > 0}
    <span>Locations:</span>
    {#each ingredient.locations as location}
        <span>{location.aisleName} - {location.bay}</span>
        <br>
    {/each}
{:else}
    <span>Locations: (None)</span>
{/if}


<FormGroup floating label="Bay">
    <Input type="select" placeholder={BayType.Types[0]}>
        {#each BayType.Types as type}
            <option value="{type}">{type}</option>
        {/each}
    </Input>
</FormGroup>