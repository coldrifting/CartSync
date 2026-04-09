<script lang="ts">
    interface Props {
        title: string;
        isOpen: boolean;
    }
    
    let {title, isOpen = $bindable()}: Props = $props();
</script>

<div class="d-flex flex-row align-items-center justify-content-between header">
    <h5 class="modal-title me-auto">{title}</h5>
    <button type="button" onclick={() => isOpen = false} class="btn-close p-2" aria-label='close'>
    </button>
</div>