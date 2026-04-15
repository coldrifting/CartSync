<script lang="ts">
    import ModalCustom from "$lib/components/modal/ModalCustom.svelte";
    import {del, patch} from "$lib/functions/requests.js";
    import FormInputTextArea from "$lib/components/FormInputTextArea.svelte";
    import {useQueryClient} from "@tanstack/svelte-query";

    interface Props {
        recipeId: string;
    }

    let {recipeId}: Props = $props();
    
    let title = $state("Update Recipe Step");
    let isOpen: boolean = $state(false);
    
    const client = useQueryClient()
    
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
    
    let isLoading: boolean = $state(false);
    async function onUpdate() {
        isLoading = true;
        await patch(`/api/recipes/steps/${stepId}/edit`, {'/Content': content, '/IsImage': isImage});
        await client.invalidateQueries({queryKey: ['recipes', recipeId]});
        isLoading = false;
        isOpen = false;
    }
    
    async function onDelete() {
        title = "Deleting Recipe Step...";
        isLoading = true;
        await del(`/api/recipes/steps/${stepId}/delete`);
        await client.invalidateQueries({queryKey: ['recipes', recipeId]});
        isLoading = false;
        isOpen = false;
    }
    
    let firstElement: HTMLTextAreaElement | undefined = $state(undefined);
</script>


<ModalCustom bind:title
             bind:isOpen
             bind:isLoading
             action={{label: "Update", action: onUpdate}}
             actionIsDisabled={content.trim() === ""}
             actionDelete={{label: "Delete", action: onDelete}}
             autoFocusElement={firstElement}
             isExpanded={true}>
    <FormInputTextArea id="inputAddStep" 
                       label="Step Details or Image URL" 
                       bind:element={firstElement} 
                       bind:value={content} 
                       rows={5}
                       required/>
</ModalCustom>