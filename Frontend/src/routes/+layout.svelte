<script lang="ts">
    import '$lib/styles/app.css';
    import {page} from '$app/state';
    import favicon from '$lib/assets/favicon.svg';
    import Appbar from "$lib/components/navigation/Appbar.svelte";
    import Sidebar from "$lib/components/navigation/Sidebar.svelte";
    import {Container} from "@sveltestrap/sveltestrap";

    let {children} = $props();

    let storesIcon: string = "M160-720v-80h640v80H160Zm0 560v-240h-40v-80l40-200h640l40 200v80h-40v240h-80v-240H560v240H160Zm80-80h240v-160H240v160Zm-38-240h556-556Zm0 0h556l-24-120H226l-24 120Z";
    let itemsIcon: string = "M280-600v-80h560v80H280Zm0 160v-80h560v80H280Zm0 160v-80h560v80H280ZM160-600q-17 0-28.5-11.5T120-640q0-17 11.5-28.5T160-680q17 0 28.5 11.5T200-640q0 17-11.5 28.5T160-600Zm0 160q-17 0-28.5-11.5T120-480q0-17 11.5-28.5T160-520q17 0 28.5 11.5T200-480q0 17-11.5 28.5T160-440Zm0 160q-17 0-28.5-11.5T120-320q0-17 11.5-28.5T160-360q17 0 28.5 11.5T200-320q0 17-11.5 28.5T160-280Z";
    let recipeIcon: string = "M222-200 80-342l56-56 85 85 170-170 56 57-225 226Zm0-320L80-662l56-56 85 85 170-170 56 57-225 226Zm298 240v-80h360v80H520Zm0-320v-80h360v80H520Z";
    let cartIcon: string = "M223.5-103.5Q200-127 200-160t23.5-56.5Q247-240 280-240t56.5 23.5Q360-193 360-160t-23.5 56.5Q313-80 280-80t-56.5-23.5Zm400 0Q600-127 600-160t23.5-56.5Q647-240 680-240t56.5 23.5Q760-193 760-160t-23.5 56.5Q713-80 680-80t-56.5-23.5ZM246-720l96 200h280l110-200H246Zm-38-80h590q23 0 35 20.5t1 41.5L692-482q-11 20-29.5 31T622-440H324l-44 80h480v80H280q-45 0-68-39.5t-2-78.5l54-98-144-304H40v-80h130l38 80Zm134 280h280-280Z";

    let navLinks: NavInfo[] = $state([
        {url: "/stores", name: "Stores", icon: storesIcon},
        {url: "/ingredients", name: "Ingredients", icon: itemsIcon},
        {url: "/recipes", name: "Recipes", icon: recipeIcon},
        {url: "/cart", name: "Cart", icon: cartIcon},
    ])

    let showNavigation: boolean = $derived(!page.url.pathname.startsWith("/login"))
</script>

<svelte:head>
    <link rel="icon" href={favicon}/>
</svelte:head>

{#if showNavigation}
    <div id="app-container">
        <Sidebar navLinks={navLinks} />

        <main id="app-content">
            <div id="content-container">
                {@render children()}
            </div>
        </main>

        <Appbar navLinks={navLinks} />

    </div>
{:else}
    {@render children()}
{/if}