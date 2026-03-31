<script lang="ts">
    import {enhance} from '$app/forms';
    import {Modal, ModalFooter, Button} from "@sveltestrap/sveltestrap";
    import {tick} from "svelte";
    
    interface Usages {
        recipes: string[];
        items: string[];
        preps: string[];
    }
    
    interface Props {
        action: string;
        header: string;
        warning: string;
    }
    
    let {action, header, warning}: Props = $props();
    
    const maxUsagesListed = 3;
    
    let id: string = $state('');
    let itemName: string | undefined = $state(undefined);
    
    let usages: Usages | undefined = $state(undefined);
    
    let isOpen: boolean = $state(false);
    
    let warningSplit = $derived(warning.split('[Name]'))
    
    const focus = () => {
        if (isOpen) {
            tick().then(() => {
                document.getElementById("inputDeleteCancel")?.focus();
            })
        }
    }
    
    const toggle = () => {
        isOpen = !isOpen;
    }
    
    export const show = (inputId: string, inputName: string | undefined = undefined, inputUsages: Record<string, string[]> | undefined = undefined) => {
        id = inputId;
        itemName = inputName;
        
        usages = inputUsages === undefined ? undefined : {
            recipes: truncateArray(inputUsages?.['Recipes'] ?? []),
            items: truncateArray(inputUsages?.['Items'] ?? []),
            preps: truncateArray(inputUsages?.['Preps'] ?? [])
        }
        
        isOpen = true;
        focus();
    }
    
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

<Modal body header={header}
       isOpen={isOpen}
       toggle={toggle}
       centered={true}>
    <form method="POST"
          action="?/{action}"
          id={action}
          use:enhance={() => {isOpen = false}}>
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
            <Button color="secondary" type="button" onclick={toggle} id="inputDeleteCancel">Cancel</Button>
            <Button color="danger" type="submit">Delete</Button>
        </ModalFooter>
    </form>
</Modal>