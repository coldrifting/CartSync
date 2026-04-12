<script lang="ts">
    import {FormGroup, Input} from "@sveltestrap/sveltestrap";
    import ModalCustom from "$lib/components/modal/ModalCustom.svelte";
    import {del, patch, put} from "$lib/functions/requests.js";
    import {invalidateAll} from "$app/navigation";

    let isOpen: boolean = $state(false);
    
    let stepId: string = $state('');
    let content: string = $state('');
    let isImage: boolean = $derived.by(() => {
        return content.trim().split(' ').length === 1 && content.includes('/');
    });

    export const show = (inputStepId: string, inputContent: string) => {
        stepId = inputStepId;
        content = inputContent;
        isOpen = true;
    }
    
    async function onUpdate() {
        await patch(`/api/recipes/steps/${stepId}/edit`, {'/Content': content, '/IsImage': isImage})
        
        isOpen = false
        await invalidateAll();
    }
    
    async function onDelete() {
        await del(`/api/recipes/steps/${stepId}/delete`);

        isOpen = false
        await invalidateAll();
    }
    
    function onOpen() {
        document.getElementById('stepContentsInput')?.focus();
    }
</script>

<ModalCustom title="Update Recipe Step"
             bind:isOpen
             action={{label: "Update", action: onUpdate}}
             actionIsDisabled={content.trim() === ""}
             actionDelete={{label: "Delete", action: onDelete}}
             onOpen={onOpen}>
    <FormGroup floating label="Step Details or Image URL">
        <Input id="stepContentsInput" name="stepContents" type="textarea" class="text-area" rows={5} bind:value={content} required/>
    </FormGroup>
</ModalCustom>