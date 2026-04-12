<script lang="ts">
    import {page} from '$app/state';
    import {Button, Card} from "@sveltestrap/sveltestrap";
    import {goto} from "$app/navigation";

    let appIcon: string = "M160-160v-80h110l-16-14q-52-46-73-105t-21-119q0-111 66.5-197.5T400-790v84q-72 26-116 88.5T240-478q0 45 17 87.5t53 78.5l10 10v-98h80v240H160Zm400-10v-84q72-26 116-88.5T720-482q0-45-17-87.5T650-648l-10-10v98h-80v-240h240v80H690l16 14q49 49 71.5 106.5T800-482q0 111-66.5 197.5T560-170Z";
    let logoutIcon: string = "M200-120q-33 0-56.5-23.5T120-200v-560q0-33 23.5-56.5T200-840h280v80H200v560h280v80H200Zm440-160-55-58 102-102H360v-80h327L585-622l55-58 200 200-200 200Z";

    interface Props {
        navLinks: NavInfo[];
    }

    let {navLinks}: Props = $props();
    
    async function logout() {
        await fetch(`/api/user/logout/cookie`, {
            method: 'POST',
            body: JSON.stringify({}),
            headers: {'Content-Type': 'application/json'},
        });

        await goto('/login')
    }
</script>

<nav>
    <Card>
        <div>
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 -960 960 960" width="32px" height="32px" fill="currentColor">
                <path d="{appIcon}"/>
            </svg>
            <span>CartSync</span>
        </div>
        <hr>
        {#each navLinks as link}
            <a href={link.url}
               aria-current={page.url.pathname.startsWith(link.url)}
               aria-label={link.name}
               class={page.url.pathname.startsWith(link.url) ? 'current' : ''}>
                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 -960 960 960" fill="currentColor">
                    <path d="{link.icon}"/>
                </svg>
                <span>{link.name}</span>
            </a>
        {/each}
        <hr>
        <div id="sidebar-spacer">

        </div>
        <hr>
        <Button color=danger block onclick={logout}>
            <div>
                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 -960 960 960" fill="currentColor">
                    <path d="{logoutIcon}"/>
                </svg>
                <span>Logout</span>
            </div>
        </Button>
    </Card>
</nav>