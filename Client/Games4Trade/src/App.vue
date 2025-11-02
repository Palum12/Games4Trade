<template>
  <div id="app">
    <Navbar class="mb-4" />
    <RouterView style="min-height: 86vh" />
    <div v-show="isSpinnerLoading" class="overlay">
      <div class="loading-spinner">
        <LoadingSpinner />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted } from 'vue'
import { useStore } from 'vuex'
import { HubConnectionBuilder } from '@microsoft/signalr'

import Navbar from './components/Navbar.vue'
import LoadingSpinner from './components/common/LoadingSpinner.vue'

const store = useStore()
const isSpinnerLoading = computed(() => store.getters.isSpinnerLoading as boolean)
const messageHubUrl = import.meta.env.VITE_MESSAGE_HUB_URL as string | undefined

onMounted(async () => {
  await store.dispatch('tryAutoLogin')

  const tokenWithoutHeader = store.getters.getTokenWithoutHeader as string | null | undefined
  if (tokenWithoutHeader && messageHubUrl) {
    const hubConnection = new HubConnectionBuilder()
      .withUrl(messageHubUrl, {
        accessTokenFactory: () => tokenWithoutHeader
      })
      .withAutomaticReconnect()
      .build()

    hubConnection.on('Recieve', (value) => {
      console.debug('Message received from hub', value)
    })

    hubConnection.start().catch((error) => {
      console.error('Failed to connect to message hub', error)
    })
  }

  void store.dispatch('getGenres')
  void store.dispatch('getSystems')
  void store.dispatch('getRegions')
  void store.dispatch('getStates')
})
</script>

<style lang="scss">
#app {
  font-family: 'Avenir', Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  background-color: whitesmoke;
  min-height: 100vh;
}

.form {
  background-color: whitesmoke;
  border: solid 1px #26bba6;
}

.overlay {
  background: rgba(255, 255, 255, 0.4);
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
}

.loading-spinner {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
}
</style>
