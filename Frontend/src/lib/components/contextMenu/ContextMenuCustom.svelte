<script lang="ts">
    import {ContextMenu} from "bits-ui";

    interface Props {
        children: any;
        contextActions?: ContextAction[] | undefined;
        id: string;
        name: string;
    }
  
    let {children, contextActions, id, name}: Props = $props();

    let open = $state(false);
</script>

<!-- TODO - Hide on shift-click, or get rid of context menu altogether? -->
<ContextMenu.Root bind:open>
    <ContextMenu.Trigger>
        {@render children()}
    </ContextMenu.Trigger>
    <ContextMenu.Portal>
        <ContextMenu.Content class="context-menu">
            {#each contextActions as action}
                <ContextMenu.Item class="context-menu-item {action.label === 'Delete' || action.label === 'Remove' ? 'destructive' : ''}"
                                  textValue={action.label}
                                  onSelect={() => action.action(id, name)}>
                    <div>
                        <span>{action.label}</span>
                    </div>
                </ContextMenu.Item>
            {/each}
        </ContextMenu.Content>
    </ContextMenu.Portal>
</ContextMenu.Root>