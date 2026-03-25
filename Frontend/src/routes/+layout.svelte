<script lang="ts">
    import {page} from '$app/state';
    import '../app.css';
    import favicon from '$lib/assets/favicon.svg';
    import {Button, Card, Col, Container, Row} from "@sveltestrap/sveltestrap";

    let {children} = $props();

    let appIcon: string = "M160-160v-80h110l-16-14q-52-46-73-105t-21-119q0-111 66.5-197.5T400-790v84q-72 26-116 88.5T240-478q0 45 17 87.5t53 78.5l10 10v-98h80v240H160Zm400-10v-84q72-26 116-88.5T720-482q0-45-17-87.5T650-648l-10-10v98h-80v-240h240v80H690l16 14q49 49 71.5 106.5T800-482q0 111-66.5 197.5T560-170Z";

    let storesIcon: string = "M160-720v-80h640v80H160Zm0 560v-240h-40v-80l40-200h640l40 200v80h-40v240h-80v-240H560v240H160Zm80-80h240v-160H240v160Zm-38-240h556-556Zm0 0h556l-24-120H226l-24 120Z";
    let itemsIcon: string = "M280-600v-80h560v80H280Zm0 160v-80h560v80H280Zm0 160v-80h560v80H280ZM160-600q-17 0-28.5-11.5T120-640q0-17 11.5-28.5T160-680q17 0 28.5 11.5T200-640q0 17-11.5 28.5T160-600Zm0 160q-17 0-28.5-11.5T120-480q0-17 11.5-28.5T160-520q17 0 28.5 11.5T200-480q0 17-11.5 28.5T160-440Zm0 160q-17 0-28.5-11.5T120-320q0-17 11.5-28.5T160-360q17 0 28.5 11.5T200-320q0 17-11.5 28.5T160-280Z";
    let recipeIcon: string = "M222-200 80-342l56-56 85 85 170-170 56 57-225 226Zm0-320L80-662l56-56 85 85 170-170 56 57-225 226Zm298 240v-80h360v80H520Zm0-320v-80h360v80H520Z";
    let cartIcon: string = "M223.5-103.5Q200-127 200-160t23.5-56.5Q247-240 280-240t56.5 23.5Q360-193 360-160t-23.5 56.5Q313-80 280-80t-56.5-23.5Zm400 0Q600-127 600-160t23.5-56.5Q647-240 680-240t56.5 23.5Q760-193 760-160t-23.5 56.5Q713-80 680-80t-56.5-23.5ZM246-720l96 200h280l110-200H246Zm-38-80h590q23 0 35 20.5t1 41.5L692-482q-11 20-29.5 31T622-440H324l-44 80h480v80H280q-45 0-68-39.5t-2-78.5l54-98-144-304H40v-80h130l38 80Zm134 280h280-280Z";
    let logoutIcon: string = "M200-120q-33 0-56.5-23.5T120-200v-560q0-33 23.5-56.5T200-840h280v80H200v560h280v80H200Zm440-160-55-58 102-102H360v-80h327L585-622l55-58 200 200-200 200Z";

    let navLinks: NavInfo[] = $state([
        {url: "/stores", name: "Stores", icon: storesIcon},
        {url: "/ingredients", name: "Ingredients", icon: itemsIcon},
        {url: "/recipes", name: "Recipes", icon: recipeIcon},
        {url: "/cart", name: "Cart", icon: cartIcon},
    ])

    let isOnLoginPage: boolean = $derived(!page.url.pathname.startsWith("/login"))
</script>

<svelte:head>
    <link rel="icon" href={favicon}/>
</svelte:head>

{#if isOnLoginPage}
    <div class="d-flex">
        <div id="sidebar-padding" class="flex-column vh-100 first-div-pad d-none d-sm-block">
        </div>
        <nav id="sidebar" class="vh-100 first-div d-none d-sm-block">
            <Card class="vh-100 border-top-0 border-bottom-0 border-left-0 rounded-0">
                <div class="nav-item nav-item-appicon">
                    <svg xmlns="http://www.w3.org/2000/svg"
                         viewBox="0 -960 960 960"
                         class="mb-1"
                         width="32px"
                         height="32px">
                        <path d="{appIcon}"/>
                    </svg>
                    <span>CartSync</span>
                </div>
                <hr class="nav-divider">
                {#each navLinks as link}
                    <a href={link.url}
                       aria-current={page.url.pathname.startsWith(link.url)}
                       aria-label={link.name}
                       class="nav-item nav-link {page.url.pathname.startsWith(link.url) ? 'active' : ''}">
                        <svg xmlns="http://www.w3.org/2000/svg"
                             viewBox="0 -960 960 960"
                             class="mb-1"
                             width="32px"
                             height="32px">
                            <path d="{link.icon}"/>
                        </svg>
                        <span>{link.name}</span>
                    </a>
                {/each}
                <hr class="nav-divider">
                <div class="nav-spacer">

                </div>
                <hr class="nav-divider">
                <form method="POST" action="/logout">
                    <Button color=danger class="nav-item logout-btn d-flex">
                        <div>
                            <svg xmlns="http://www.w3.org/2000/svg"
                                 viewBox="0 -960 960 960"
                                 width="32px"
                                 height="32px">
                                <path d="{logoutIcon}"/>
                            </svg>
                            <span>Logout</span>
                        </div>
                    </Button>
                </form>
            </Card>
        </nav>

        <div class="main-content vh-100 second-div">
            <div class="container-fluid test-content overflow-scroll">
                <Container>
                    <Row>
                        <Col lg="10" xl="8" class="offset-lg-1 offset-xl-2">
                            <div class="pt-4">
                            </div>
                            {@render children()}
                            <div class="pb-4">
                            </div>
                        </Col>
                    </Row>
                </Container>
            </div>

            <div id="footer" class="test-footer-container">
                <Card>
                    <Container>
                        <Row>
                            {#each navLinks as navLink}
                                <Col>
                                    <a href="{navLink.url}"
                                       aria-current={page.url.pathname.startsWith(navLink.url)}
                                       class="nav-footer-item {page.url.pathname.startsWith(navLink.url) ? 'nav-footer-item-active' : ''}">
                                        <div>
                                            <svg xmlns="http://www.w3.org/2000/svg"
                                                 viewBox="0 -960 960 960"
                                                 class="content-center"
                                                 width="40px"
                                                 height="40px">
                                                <path d="{navLink.icon}"/>
                                            </svg>
                                            <span>{navLink.name}</span>
                                        </div>
                                    </a>
                                </Col>
                            {/each}
                        </Row>
                    </Container>
                </Card>
            </div>
        </div>
    </div>
{:else}
    {@render children()}
{/if}