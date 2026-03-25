<script lang="ts">
    let {url, title} = $props();
</script>

<a href="{url}" class="title-navigation">
    <div>
        <svg class="header-icon" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" viewBox="2 0 8 16">
            <path stroke="currentColor" stroke-linecap="round" stroke-width="2"
                  d="M8,16 2,8 8,0"/>
        </svg>
        <span>{title}</span>
    </div>
</a>