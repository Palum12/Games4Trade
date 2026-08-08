<template>
  <div v-show="isActive" class="tab-panel">
    <slot />
  </div>
</template>

<script setup lang="ts">
import { computed, inject, onBeforeUnmount, watch } from 'vue'

import { tabsInjectionKey } from './Tabs.vue'

const props = defineProps<{ title: string }>()

const context = inject(tabsInjectionKey)

if (!context) {
  throw new Error('TabPanel must be used inside Tabs')
}

const tabId = Symbol('tab-panel')

context.registerTab(props.title, tabId)

watch(
  () => props.title,
  (value) => {
    context.updateTitle(tabId, value)
  }
)

onBeforeUnmount(() => {
  context.unregisterTab(tabId)
})

const isActive = computed(() => context.isActive(tabId))
</script>

<style scoped>
.tab-panel {
  width: 100%;
}
</style>
