<script lang="ts">
    import {FormGroup, Input} from "@sveltestrap/sveltestrap";
    import ModalCustom from "$lib/components/modal/ModalCustom.svelte";
    import {del, patch, put} from "$lib/functions/requests.js";
    import {invalidateAll} from "$app/navigation";
    import FormInputText from "$lib/components/FormInputText.svelte";

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
    
    let firstElement: HTMLTextAreaElement | undefined = $state(undefined);
</script>


<ModalCustom title="Update Recipe Step"
             bind:isOpen
             action={{label: "Update", action: onUpdate}}
             actionIsDisabled={content.trim() === ""}
             autoFocusElement={firstElement}
             isExpanded={true}>
    <FormInputText id="inputAddStep" 
                   label="Step Details or Image URL" 
                   bind:element={firstElement} 
                   bind:value={content} 
                   rows={5}
                   required/>
</ModalCustom>