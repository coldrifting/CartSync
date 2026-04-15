<script lang="ts">
    import ItemTemp from "$lib/models/ItemTemp.js";
    import {FormGroup, Input} from "@sveltestrap/sveltestrap";
    import {invalidateAll} from "$app/navigation";
    import {patch} from "$lib/functions/requests.js";

    interface Props {
        itemId: string;
        itemTemp: string;
    }

    let {itemId, itemTemp}: Props = $props();

    async function onchange() {
        try {
            await patch(`/api/items/${itemId}/edit`, {"/Temp": itemTemp});
        }
        catch(error) {
            console.error(error);
            await invalidateAll();
        }
    }
</script>

<FormGroup floating label="Temperature">
    <Input type="select"
           name="itemTemp"
           bind:value={itemTemp}
           onchange={onchange}>
        {#each ItemTemp.Temps as temp}
            <option value="{temp}">{temp}</option>
        {/each}
    </Input>
</FormGroup>