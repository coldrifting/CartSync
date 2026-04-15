<script lang="ts">
    import UnitType from "$lib/models/UnitType.js";
    import {FormGroup, Input} from "@sveltestrap/sveltestrap";
    import {invalidateAll} from "$app/navigation";
    import {patch} from "$lib/functions/requests.js";

    interface Props {
        itemId: string;
        itemDefaultUnitType: string;
    }

    let {itemId, itemDefaultUnitType}: Props = $props();

    async function onchange() {
        try {
            await patch(`/api/items/${itemId}/edit`, {"/DefaultUnitType": itemDefaultUnitType});
        }
        catch(error) {
            console.error(error);
            await invalidateAll();
        }
    }
</script>

<FormGroup floating label="Default Units">
    <Input type="select"
           name="itemDefaultUnits"
           bind:value={itemDefaultUnitType}
           onchange={onchange}>
        {#each UnitType.Types as type}
            <option value="{type}">{UnitType.asString(type)}</option>
        {/each}
    </Input>
</FormGroup>