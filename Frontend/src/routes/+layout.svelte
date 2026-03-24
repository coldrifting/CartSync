<script lang="ts">
	import "../app.css";
	
	import { page } from '$app/state';
	import favicon from '$lib/assets/favicon.svg';

	let { children } = $props();
	
	let appIcon: string = "M160-160v-80h110l-16-14q-52-46-73-105t-21-119q0-111 66.5-197.5T400-790v84q-72 26-116 88.5T240-478q0 45 17 87.5t53 78.5l10 10v-98h80v240H160Zm400-10v-84q72-26 116-88.5T720-482q0-45-17-87.5T650-648l-10-10v98h-80v-240h240v80H690l16 14q49 49 71.5 106.5T800-482q0 111-66.5 197.5T560-170Z";
	
	let storesIcon: string = "M160-720v-80h640v80H160Zm0 560v-240h-40v-80l40-200h640l40 200v80h-40v240h-80v-240H560v240H160Zm80-80h240v-160H240v160Zm-38-240h556-556Zm0 0h556l-24-120H226l-24 120Z";
	let itemsIcon: string = "M280-600v-80h560v80H280Zm0 160v-80h560v80H280Zm0 160v-80h560v80H280ZM160-600q-17 0-28.5-11.5T120-640q0-17 11.5-28.5T160-680q17 0 28.5 11.5T200-640q0 17-11.5 28.5T160-600Zm0 160q-17 0-28.5-11.5T120-480q0-17 11.5-28.5T160-520q17 0 28.5 11.5T200-480q0 17-11.5 28.5T160-440Zm0 160q-17 0-28.5-11.5T120-320q0-17 11.5-28.5T160-360q17 0 28.5 11.5T200-320q0 17-11.5 28.5T160-280Z";
	let recipeIcon: string = "M222-200 80-342l56-56 85 85 170-170 56 57-225 226Zm0-320L80-662l56-56 85 85 170-170 56 57-225 226Zm298 240v-80h360v80H520Zm0-320v-80h360v80H520Z";
	let cartIcon: string = "M223.5-103.5Q200-127 200-160t23.5-56.5Q247-240 280-240t56.5 23.5Q360-193 360-160t-23.5 56.5Q313-80 280-80t-56.5-23.5Zm400 0Q600-127 600-160t23.5-56.5Q647-240 680-240t56.5 23.5Q760-193 760-160t-23.5 56.5Q713-80 680-80t-56.5-23.5ZM246-720l96 200h280l110-200H246Zm-38-80h590q23 0 35 20.5t1 41.5L692-482q-11 20-29.5 31T622-440H324l-44 80h480v80H280q-45 0-68-39.5t-2-78.5l54-98-144-304H40v-80h130l38 80Zm134 280h280-280Z";
	let logoutIcon: string = "M200-120q-33 0-56.5-23.5T120-200v-560q0-33 23.5-56.5T200-840h280v80H200v560h280v80H200Zm440-160-55-58 102-102H360v-80h327L585-622l55-58 200 200-200 200Z";
	
	let navLinks: NavInfo[] = $state([
		{ url: "/", name: "CartSync", icon: appIcon },	
		{ url: "", name: "Rule", icon: "" },	
		{ url: "/stores", name: "Stores", icon: storesIcon },
		{ url: "/ingredients", name: "Ingredients", icon: itemsIcon },
		{ url: "/recipes", name: "Recipes", icon: recipeIcon },
		{ url: "/cart", name: "Cart", icon: cartIcon },
		{ url: "", name: "Rule", icon: "" },	
		{ url: "", name: "Spacer", icon: "" },	
		{ url: "", name: "Rule", icon: "" },	
		{ url: "/logout", name: "Logout", icon: logoutIcon }
	])
</script>

<svelte:head>
	<link rel="icon" href={favicon} />
</svelte:head>

<div class="flex flex-col h-screen justify-between">
	
	<div class="group bg-gray-800 h-screen absolute z-50 left-0 hidden sm:block">
		<nav class="ml-2 mr-2 flex-1 flex flex-col h-full">
			<div class="mt-3">
				
			</div>
		{#each navLinks as navLink}
			{#if navLink.name === "CartSync"}
				<div
					class="
					flex flex-row
					p-2
					pt-1
					pb-1
					mb-3
					h-12
					w-12
					group-hover:w-48
					
					text-gray-200
					fill-gray-200
">
						<svg xmlns="http://www.w3.org/2000/svg" 
							 viewBox="0 -960 960 960" 
							 class="w-8 mt-1"
							 width="32px" 
							 height="32px">
							<path d="{navLink.icon}"/>
						</svg>
						<p class="hidden group-hover:block ml-4 mt-2 align-middle">{navLink.name}</p>
				</div>
			{:else if navLink.name === "Rule"}
				<hr class="mb-3 border-gray-500">
			{:else if navLink.name === "Spacer"}
				<div class="flex-1">
					
				</div>
			{:else if navLink.name === "Logout"}
				<form method="POST" action="/logout">
					<button
						aria-label="Logout"
						class="
						flex flex-row
						p-2
						pt-1
						pb-1
						mb-3
						h-12
						w-12
						hover:cursor-pointer
						group-hover:w-48
						rounded-lg
						
						text-gray-200
						fill-gray-200
						
						hover:bg-red-700
						hover:fill-white 
						hover:text-white 
						
						active:bg-red-300
	">
							<svg xmlns="http://www.w3.org/2000/svg" 
								 viewBox="0 -960 960 960" 
								 class="w-8 mt-1"
								 width="32px" 
								 height="32px">
								<path d="{navLink.icon}"/>
							</svg>
							<span class="hidden group-hover:block ml-4 mt-2 align-middle">Logout</span>
					</button>
				</form>
			{:else}
				<a href="{navLink.url}" 
					aria-current={page.url.pathname.startsWith(navLink.url)}
					aria-label={navLink.name}
				   	
					class="
					flex flex-row
					p-2
					pt-1
					pb-1
					mb-3
					h-12
					w-12
					group-hover:w-48
					rounded-lg
					
					text-gray-200
					fill-gray-200
					
					aria-current:bg-amber-600
					aria-current:text-white
					aria-current:fill-white
					
					hover:bg-amber-700
					hover:fill-white 
					hover:text-white 
					
					aria-current:hover:text-white
					aria-current:hover:fill-white
					aria-current:hover:bg-amber-700
					
					active:bg-blue-500
					aria-current:active:bg-blue-500
">
						<svg xmlns="http://www.w3.org/2000/svg" 
							 viewBox="0 -960 960 960" 
							 class="w-8 mt-1"
							 width="32px" 
							 height="32px">
							<path d="{navLink.icon}"/>
						</svg>
						{#if !navLink.name.startsWith("[")}
							<p class="hidden group-hover:block ml-4 mt-2 align-middle">{navLink.name}</p>
						{/if}
				</a>
			{/if}
		{/each}
		</nav>
	</div>
	
	<div class="flex-1 overflow-scroll sm:pl-20 md:pr-20">
		<div class="flex flex-row">
			<div class="flex-1 max-w-2xl mx-auto">
				<div class="flex-1 flex flex-col p-5 pb-10">
					{@render children()}
				</div>
			</div>
		</div>
	</div>
	
	<!--Mobile App Bar Footer-->
	<footer class="bg-gray-800 flex sm:hidden">
		<div class="flex flex-1 max-w-2xl mx-auto">
			<nav class="pt-2 mb-2.5 flex-1 flex flex-row items-center justify-around">
			{#each navLinks as navLink}
				{#if navLink.name !== "Rule" &&
					 navLink.name !== "Spacer" &&
					 navLink.name !== "Logout" &&
					 navLink.name !== "CartSync"}
					<a href="{navLink.url}" 
						aria-current={page.url.pathname.startsWith(navLink.url)}
						class="
						fill-white hover:amber-700 
						hover:fill-amber-700 
						hover:text-amber-700 
						aria-current:text-amber-500 
						aria-current:fill-amber-500 
						aria-current:hover:text-amber-700
						aria-current:hover:fill-amber-700"
					>
						<div class="flex justify-center">
							<svg xmlns="http://www.w3.org/2000/svg" 
								 viewBox="0 -960 960 960" 
								 class="content-center"
								 width="46px" 
								 height="46px">
								<path d="{navLink.icon}"/>
							</svg>
						</div>
						<p class="text-center text-sm font-semibold">{navLink.name}</p>
					</a>
				{/if}
			{/each}
			</nav>
		</div>
	</footer>
	
</div>