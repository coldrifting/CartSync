<script lang="ts">
    import {tick} from "svelte";
    import ModalCustom from "$lib/components/modal/ModalCustom.svelte";
    import {invalidateAll} from "$app/navigation";
    import {post} from "$lib/functions/requests.js";
    import FormInputText from "$lib/components/FormInputText.svelte";

    interface Props {
        recipeId: string;
    }

    let {recipeId}: Props = $props();

    let isOpen: boolean = $state(false);
    
    let content: string = $state('');
    let isImage: boolean = $derived.by(() => {
        return content.trim().split(' ').length === 1 && content.includes('/');
    });
    
    export function show() {
        content = '';
        isOpen = true;
    }
    
    async function onAdd() {
        await post(`/api/recipes/steps/add?recipeId=${recipeId}`, {content: content, isImage: isImage})

        isOpen = false
        await invalidateAll();
        tick().then(() => {
            window.scrollTo(0, document.body.scrollHeight);
        });
    }
    
    let firstElement: HTMLTextAreaElement | undefined = $state(undefined);
</script>

<ModalCustom title="Add Recipe Step"
             bind:isOpen
             action={{label: "Add", action: onAdd}}
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