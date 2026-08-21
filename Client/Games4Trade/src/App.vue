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
import { computed, onBeforeUnmount, onMounted, watch, type WatchStopHandle } from 'vue'
import { useStore } from 'vuex'

import Navbar from './components/Navbar.vue'
import LoadingSpinner from './components/common/LoadingSpinner.vue'
import { connectToMessageHub, disconnectFromMessageHub } from './services/messageHub'

const store = useStore()
const isSpinnerLoading = computed(() => store.getters.isSpinnerLoading as boolean)
const messageHubUrl = import.meta.env.VITE_MESSAGE_HUB_URL as string | undefined
let stopWatchingToken: WatchStopHandle | undefined

onMounted(async () => {
  await store.dispatch('tryAutoLogin')

  stopWatchingToken = watch(
    () => store.getters.getTokenWithoutHeader as string | null | undefined,
    (tokenWithoutHeader) => {
      if (!tokenWithoutHeader || !messageHubUrl) {
        void disconnectFromMessageHub()
        return
      }

      void connectToMessageHub(messageHubUrl, tokenWithoutHeader).catch((error: unknown) => {
        console.error('Failed to connect to message hub', error)
      })
    },
    { immediate: true }
  )

  void store.dispatch('getGenres')
  void store.dispatch('getSystems')
  void store.dispatch('getRegions')
  void store.dispatch('getStates')
})

onBeforeUnmount(() => {
  stopWatchingToken?.()
  void disconnectFromMessageHub()
})
</script>

<style lang="scss">
#app {
  font-family: 'Avenir', Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  background-color: whitesmoke;
  min-height: 100vh;
  overflow-x: hidden;
}

.form {
  background-color: whitesmoke;
  border: solid 1px #26bba6;
}

.form-group {
  margin-bottom: 1rem;
}

/* Bootstrap 5 no longer defines the Bootstrap 4 helper used by legacy forms. */
.form-row {
  display: flex;
  flex-wrap: wrap;
  margin-right: -0.25rem;
  margin-left: -0.25rem;
}

.form-row > [class*='col-'] {
  padding-right: 0.25rem;
  padding-left: 0.25rem;
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
