<script lang="ts">
    import ModalCustom from "$lib/components/modal/ModalCustom.svelte";
    
    interface Usages {
        recipes: string[];
        items: string[];
        preps: string[];
    }
    
    interface Props {
        type: string;
        warning: string;
        deleteAction: (id: string) => Promise<void>;
        parentIsOpen?: boolean;
    }
    
    let {type, warning, deleteAction, parentIsOpen = $bindable(undefined)}: Props = $props();
    
    const maxUsagesListed = 3;
    
    let id: string = $state('');
    let itemName: string | undefined = $state(undefined);
    
    let usages: Usages | undefined = $state(undefined);
    
    let isOpen: boolean = $state(false);
    
    let warningSplit = $derived(warning.split('[Name]'))
    
    export function show(inputId: string, inputName: string | undefined = undefined, inputUsages: Record<string, string[]> | undefined = undefined) {
        id = inputId;
        itemName = inputName;
        
        usages = inputUsages === undefined ? undefined : {
            recipes: truncateArray(inputUsages?.['Recipes'] ?? []),
            items: truncateArray(inputUsages?.['Items'] ?? []),
            preps: truncateArray(inputUsages?.['Preps'] ?? [])
        }
        
        isOpen = true;
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
    
    let isLoading: boolean = $state(false);
    async function onDelete() {
        isLoading = true;
        await deleteAction(id);
        isLoading = false;
        isOpen = false;
        if (parentIsOpen) {
            parentIsOpen = false;
        }
    }
</script>

<ModalCustom title="Delete {type}" 
             bind:isOpen 
             bind:isLoading 
             actionDelete={{label: "Delete", action: onDelete}}>
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
</ModalCustom>