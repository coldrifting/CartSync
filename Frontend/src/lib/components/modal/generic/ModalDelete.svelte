<script lang="ts">
    import {enhance} from '$app/forms';
    import {Modal, ModalFooter, Button} from "@sveltestrap/sveltestrap";
    import type {SubmitFunction} from "@sveltejs/kit";
    import {trapFocus} from 'trap-focus-svelte'
    import ModalHeaderCustom from "$lib/components/modal/ModalHeaderCustom.svelte";
    
    interface Usages {
        recipes: string[];
        items: string[];
        preps: string[];
    }
    
    interface Props {
        type: string;
        warning: string;
        parentIsOpen?: boolean | undefined;
    }
    
    let {type, warning, parentIsOpen = $bindable(undefined)}: Props = $props();
    
    const maxUsagesListed = 3;
    
    let id: string = $state('');
    let itemName: string | undefined = $state(undefined);
    
    let usages: Usages | undefined = $state(undefined);
    
    let isOpen: boolean = $state(false);
    
    let warningSplit = $derived(warning.split('[Name]'))
    
    export const show = (inputId: string, inputName: string | undefined = undefined, inputUsages: Record<string, string[]> | undefined = undefined) => {
        id = inputId;
        itemName = inputName;
        
        usages = inputUsages === undefined ? undefined : {
            recipes: truncateArray(inputUsages?.['Recipes'] ?? []),
            items: truncateArray(inputUsages?.['Items'] ?? []),
            preps: truncateArray(inputUsages?.['Preps'] ?? [])
        }
        
        isOpen = true;
    }

    const submitFunction: SubmitFunction = () => {
        return async ({update}) => {
            isOpen = false
            if (parentIsOpen) {
                parentIsOpen = false;
            }
            await update({reset: false});
        };
    };
    
    function truncateArray(arr: string[]): string[] {
      if (arr.length <= maxUsagesListed) {
        return arr;
      }
      return arr.slice(0, maxUsagesListed).concat('...');
    }
    
    function toTitleCase(str: string): string {
        return str.charAt(0).toUpperCase() + str.slice(1);
    }
</script>

<Modal body
       isOpen={isOpen}
       toggle={() => isOpen = !isOpen}
       centered={true}
       on:open={() => document.getElementById("inputDeleteCancel")?.focus()}>
    <form method="POST"
          action="?/delete{type}"
          use:enhance={submitFunction}
          use:trapFocus={true}>
        <ModalHeaderCustom title="Delete {type}" bind:isOpen={isOpen}/>
        <div>
            <input hidden name="id" bind:value={id} required />
            <p>
            {#if usages !== undefined}
                {itemName} is currently used in:
                {#each Object.entries(usages) as usage}
                    {#if usage[1].length > 0}
                        <br>
                        <span class="text-warning">{toTitleCase(usage[0])}:</span><br>
                        {#each usage[1] as c}
                            {c}<br>
                        {/each}
                    {/if}
                {/each}
            {:else}
                {#if warningSplit.length === 2}
                    {warningSplit[0]}
                    <span class="text-warning">{itemName}</span>
                    {warningSplit[1]}
                {:else}
                    {warning.replace('[Name]', itemName ?? '')}
                {/if}
            {/if}
            <br>Are you sure?</p>
        </div>
        <ModalFooter>
            <Button color="secondary" type="button" onclick={() => isOpen = false} id="inputDeleteCancel">Cancel</Button>
            <Button color="danger" type="submit">Delete</Button>
        </ModalFooter>
    </form>
</Modal>