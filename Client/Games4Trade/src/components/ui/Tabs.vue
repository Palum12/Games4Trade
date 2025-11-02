<template>
  <div class="tabs">
    <div class="tab-list">
      <button
        v-for="(tab, index) in tabs"
        :key="tab.id"
        type="button"
        class="tab-button"
        :class="{ active: activeIndex === index }"
        @click="selectTab(index)"
      >
        {{ tab.title }}
      </button>
    </div>
    <div class="tab-panels">
      <slot />
    </div>
  </div>
</template>

<script setup lang="ts">
import { provide, reactive, ref, watchEffect } from 'vue'
import type { InjectionKey, Ref } from 'vue'

type TabInfo = {
  id: symbol
  title: string
}

export interface TabsContext {
  activeIndex: Ref<number>
  registerTab: (title: string, id: symbol) => void
  unregisterTab: (id: symbol) => void
  updateTitle: (id: symbol, title: string) => void
  isActive: (id: symbol) => boolean
}

export const tabsInjectionKey: InjectionKey<TabsContext> = Symbol('TabsContext')

const props = defineProps<{ defaultIndex?: number }>()

const tabs = reactive<TabInfo[]>([])
const activeIndex = ref(props.defaultIndex ?? 0)

const registerTab = (title: string, id: symbol) => {
  tabs.push({ id, title })
}

const unregisterTab = (id: symbol) => {
  const index = tabs.findIndex((tab) => tab.id === id)
  if (index !== -1) {
    tabs.splice(index, 1)
    if (activeIndex.value >= tabs.length) {
      activeIndex.value = Math.max(tabs.length - 1, 0)
    }
  }
}

const updateTitle = (id: symbol, title: string) => {
  const tab = tabs.find((item) => item.id === id)
  if (tab) {
    tab.title = title
  }
}

const isActive = (id: symbol) => tabs[activeIndex.value]?.id === id

const selectTab = (index: number) => {
  activeIndex.value = index
}

provide(tabsInjectionKey, {
  activeIndex,
  registerTab,
  unregisterTab,
  updateTitle,
  isActive
})

watchEffect(() => {
  if (tabs.length === 0) {
    activeIndex.value = 0
  }
})
</script>

<style scoped>
.tabs {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.tab-list {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
}

.tab-button {
  padding: 0.5rem 1rem;
  border: 1px solid #26bba6;
  background-color: white;
  color: #26bba6;
  border-radius: 0.25rem;
  cursor: pointer;
  transition: background-color 0.2s ease, color 0.2s ease;
}

.tab-button.active {
  background-color: #26bba6;
  color: white;
}

.tab-button:focus {
  outline: none;
  box-shadow: 0 0 0 0.2rem rgba(38, 187, 166, 0.3);
}

.tab-panels {
  width: 100%;
}
</style>
