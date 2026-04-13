<script lang="ts">
    import {page} from '$app/state';

    interface Props {
        navLinks: NavInfo[];
    }

    let {navLinks}: Props = $props();
</script>

<style>
    footer {
        height: calc(var(--appbar-height) + env(safe-area-inset-bottom));
        width: 100%;
        padding-top: 0.5rem;
        padding-bottom: calc(env(safe-area-inset-bottom));
        background-color: var(--theme-card-bg);
        border-top: 1px solid var(--theme-card-border);
        display: flex;
        flex-direction: row;
        justify-content: space-around;
        position: fixed;
        bottom: 0;
        z-index: 3;

        @media (width >= 576px) {
            display: none;
        }

        a {
            -webkit-touch-callout: none;
            text-decoration: none;
            color: var(--theme-fg);
            font-weight: 500;
            font-size: 0.75em;

            div {
                display: flex;
                flex-direction: column;
                align-items: center;
            }

            &.current {
                color: var(--nav-appbar-current);

                svg {
                    color: var(--nav-appbar-current);
                }
            }

            :active {
                color: var(--nav-appbar-pressed);

                svg {
                    color: var(--nav-appbar-pressed);
                }
            }

            :hover {
                color: var(--nav-appbar-pressed);

                svg {
                    color: var(--nav-appbar-pressed);
                }
            }
        }

        @media (width >= 576px) {
            display: none;
        }
    }
</style>

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