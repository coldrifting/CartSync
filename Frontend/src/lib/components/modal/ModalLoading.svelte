<script lang="ts">
    import {fade} from 'svelte/transition';
    import ModalCustom from "$lib/components/modal/ModalCustom.svelte";
    import LoadingPage from "$lib/components/LoadingPage.svelte";
    
    interface Props {
        title: string;
    }
    
    let {title}: Props = $props();
</script>

<div in:fade={{delay: 400, duration: 200}}>
    <ModalCustom title={title} isOpen={true} isLoading={true}>
        <span>Loading</span>
    </ModalCustom>
</div>