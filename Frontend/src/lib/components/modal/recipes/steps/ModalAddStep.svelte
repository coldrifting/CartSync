<script lang="ts">
    import {tick} from "svelte";
    import {FormGroup, Input} from "@sveltestrap/sveltestrap";
    import ModalCustom from "$lib/components/modal/ModalCustom.svelte";
    import {invalidateAll} from "$app/navigation";
    import {post} from "$lib/functions/requests.js";

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
    
    function onOpen() {
        document.getElementById('stepContentsInput')?.focus();
    }
</script>

<ModalCustom title="Add Recipe Step"
             bind:isOpen
             action={{label: "Add", action: onAdd}}
             actionIsDisabled={content.trim() === ""}
             onOpen={onOpen}>
    <FormGroup floating label="Step Details or Image URL">
        <Input id="stepContentsInput" name="stepContents" type="textarea" class="text-area" rows={5} bind:value={content} required/>
    </FormGroup>
</ModalCustom>