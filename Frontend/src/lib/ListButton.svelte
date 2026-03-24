<script lang="ts">
    let { name } = $props()
</script>

<li>
    <button type="submit"
        aria-label="{name}"
        class="
            flex 
            p-2.5
            pl-5
            pr-3
            w-full
            border-r-0
            border-l-0
            border-t-0
            bg-blue-500
            hover:bg-blue-700 
            active:bg-blue-900 
            backdrop-brightness-125
            rounded-lg"
    >
        <span class="font-semibold">{name}</span>
    </button>
</li>