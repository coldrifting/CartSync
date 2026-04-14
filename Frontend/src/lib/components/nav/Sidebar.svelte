<script lang="ts">
    import {page} from '$app/state';
    import {browser} from "$app/environment";
    import {goto} from "$app/navigation";
    import {redirect} from "@sveltejs/kit";

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

        if (browser) {
            await goto('/login');
        } else {
            redirect(307, '/login');
        }
    }
</script>

<style>
    nav {
        position: fixed;
        width: var(--sidebar-width);
        height: 100dvh;
        font-weight: 500;
        display: flex;
        flex-direction: column;
        z-index: 4;
        background-color: var(--theme-input-bg);

        @media (width < 576px) {
            display: none;
        }

        .nav-item {
            padding: 0.5rem;
            margin: 0.5rem;
            height: 3rem;
            width: calc(var(--sidebar-width) - 1rem);
            border: 1px solid transparent;
            display: flex;
            flex-direction: row;
            align-items: center;
            user-select: none;
            color: var(--theme-fg);
        }

        a {
            text-decoration: none;
            color: var(--theme-fg);
            border-radius: 0.5rem;
            padding: 0.5rem;
            margin: 0.5rem;
            height: 3rem;
            width: calc(var(--sidebar-width) - 1rem);
            border: 1px solid transparent;

            &.current {
                background-color: var(--nav-sidebar-current);
                border: 1px solid var(--nav-sidebar-highlight);
                color: white;
                
                svg {
                    color: white;
                }
            }

            &:hover {
                background-color: var(--nav-sidebar-highlight);
                border: 1px solid var(--nav-sidebar-highlight-border);
                color: white;
                
                svg {
                    color: white;
                }
            }
        }

        svg {
            color: var(--theme-fg);
            min-width: 2rem !important;
            width: 2rem !important;
        }

        hr {
            margin: 0 0.5rem !important;
        }

        span {
            @media (width < 1024px) {
                display: none;
            }
        }

        .btn {
            --bs-btn-bg: transparent !important;
            --bs-btn-border-color: transparent !important;
            
            &:hover {
                color: white;
                svg {
                    color: white;
                }
            }
        }
    }
</style>

<nav>
    <div class="nav-item">
        <span class="d-flex flex-row align-items-center m-0 p-0">
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 -960 960 960" fill="currentColor">
                <path d="{appIcon}"/>
            </svg>
            <span class="ps-3 fw-semibold">CartSync</span>
        </span>
    </div>
    <hr>
    {#each navLinks as link}
        <a href={link.url}
           aria-current={page.url.pathname.startsWith(link.url)}
           aria-label={link.name}
           class:current={page.url.pathname.startsWith(link.url)}>
            <span class="d-flex flex-row align-items-center m-0 p-0">
                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 -960 960 960" fill="currentColor">
                    <path d="{link.icon}"/>
                </svg>
                <span class="ps-3">{link.name}</span>
            </span>
        </a>
    {/each}
    <hr>
    <div class="flex-grow-1">

    </div>
    <hr>
    <button class="btn btn-danger nav-item" onclick={logout}>
        <span class="d-flex flex-row align-items-center m-0 p-0">
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 -960 960 960" fill="currentColor">
                <path d="{logoutIcon}"/>
            </svg>
            <span class="ps-3 fw-semibold">Logout</span>
        </span>
    </button>
</nav>