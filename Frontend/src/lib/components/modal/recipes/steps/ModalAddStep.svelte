<script lang="ts">
    import {tick} from "svelte";
    import ModalCustom from "$lib/components/modal/ModalCustom.svelte";
    import {post} from "$lib/functions/requests.js";
    import FormInputTextArea from "$lib/components/FormInputTextArea.svelte";
    import {useQueryClient} from "@tanstack/svelte-query";

    interface Props {
        recipeId: string;
    }

    let {recipeId}: Props = $props();

    const client = useQueryClient()
    
    let isOpen: boolean = $state(false);
    
    let content: string = $state('');
    let isImage: boolean = $derived.by(() => {
        return content.trim().split(' ').length === 1 && content.includes('/');
    });
    
    export function show() {
        content = '';
        isOpen = true;
    }
    
    let isLoading: boolean = $state(false);
    async function onAdd() {
        isLoading = true;
        await post(`/api/recipes/steps/add?recipeId=${recipeId}`, {content: content, isImage: isImage});
        await client.invalidateQueries({queryKey: ['recipes', recipeId]});
        isLoading = false;
        isOpen = false;
        
        tick().then(() => {
            window.scrollTo(0, document.body.scrollHeight);
        });
    }
    
    let firstElement: HTMLTextAreaElement | undefined = $state(undefined);
</script>

<ModalCustom title="Add Recipe Step"
             bind:isOpen
             bind:isLoading
             action={{label: "Add", action: onAdd}}
             actionIsDisabled={content.trim() === ""}
             autoFocusElement={firstElement}
             isExpanded={true}>
    <FormInputTextArea id="inputAddStep"
                       label="Step Details or Image URL"
                       bind:element={firstElement}
                       bind:value={content}
                       rows={5}
                       required/>
</ModalCustom>