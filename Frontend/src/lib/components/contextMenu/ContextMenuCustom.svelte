<script lang="ts">
  import { ContextMenu } from "bits-ui";

  let {
    children,
    actions,
    id,
    name
  }: {
    children: any,
    actions: ContextAction[],
    id: string,
    name: string
  } = $props();
  
  let open = $state(false);
</script>
 
<ContextMenu.Root bind:open>
  <ContextMenu.Trigger>
      {@render children()}
  </ContextMenu.Trigger>
  <ContextMenu.Portal>
    <ContextMenu.Content class="context-menu">
        {#each actions as action}
          <ContextMenu.Item class="context-menu-item {action.label === 'Delete' ? 'destructive' : ''}" 
                            textValue={action.label} 
                            onSelect={() => action.action(id, name)} >
            <div>
              <span>{action.label}</span>
            </div>
          </ContextMenu.Item>
        {/each}
    </ContextMenu.Content>
  </ContextMenu.Portal>
</ContextMenu.Root>