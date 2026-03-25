<script lang="ts">
    import {page} from '$app/state';

    interface Props {
        navLinks: NavInfo[];
    }

    let {navLinks}: Props = $props();
</script>

<footer>
    {#each navLinks as navLink}
        <a href="{navLink.url}"
           aria-current={page.url.pathname.startsWith(navLink.url)}
           class={page.url.pathname.startsWith(navLink.url) ? 'current' : ''}>
            <div>
                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 -960 960 960" width="40px" height="40px" fill="currentColor">
                    <path d="{navLink.icon}"/>
                </svg>
                <span>{navLink.name}</span>
            </div>
        </a>
    {/each}
</footer>