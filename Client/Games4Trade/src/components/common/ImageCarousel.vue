<template>
  <div class="carousel" role="region" aria-roledescription="carousel">
    <div class="image-wrapper">
      <img :src="currentImage" alt="Zdjęcie ogłoszenia" />
    </div>
    <div v-if="images.length > 1" class="controls">
      <button type="button" class="control" @click="previous" aria-label="Poprzednie zdjęcie">
        ‹
      </button>
      <button type="button" class="control" @click="next" aria-label="Następne zdjęcie">
        ›
      </button>
    </div>
    <div v-if="images.length > 1" class="indicators">
      <button
        v-for="(image, index) in images"
        :key="image"
        type="button"
        class="indicator"
        :class="{ active: index === activeIndex }"
        @click="goTo(index)"
        :aria-label="`Pokaż zdjęcie ${index + 1}`"
      ></button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue'

const props = defineProps<{ images: string[] }>()

const activeIndex = ref(0)

watch(
  () => props.images.length,
  (length) => {
    if (activeIndex.value >= length) {
      activeIndex.value = 0
    }
  }
)

const currentImage = computed(() => props.images[activeIndex.value] ?? '')

const next = () => {
  if (props.images.length === 0) {
    return
  }
  activeIndex.value = (activeIndex.value + 1) % props.images.length
}

const previous = () => {
  if (props.images.length === 0) {
    return
  }
  activeIndex.value = (activeIndex.value - 1 + props.images.length) % props.images.length
}

const goTo = (index: number) => {
  activeIndex.value = index
}
</script>

<style scoped>
.carousel {
  position: relative;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.75rem;
}

.image-wrapper {
  width: 100%;
  display: flex;
  justify-content: center;
}

img {
  max-height: 45vh;
  width: 100%;
  object-fit: contain;
  border-radius: 0.5rem;
  background-color: #fff;
}

.controls {
  position: absolute;
  top: 50%;
  left: 0;
  width: 100%;
  display: flex;
  justify-content: space-between;
  transform: translateY(-50%);
  pointer-events: none;
}

.control {
  background: rgba(0, 0, 0, 0.4);
  color: #fff;
  border: none;
  font-size: 2rem;
  line-height: 1;
  width: 2.5rem;
  height: 2.5rem;
  border-radius: 9999px;
  cursor: pointer;
  pointer-events: auto;
}

.indicators {
  display: flex;
  gap: 0.5rem;
}

.indicator {
  width: 12px;
  height: 12px;
  border-radius: 50%;
  border: none;
  background-color: rgba(38, 187, 166, 0.3);
  cursor: pointer;
}

.indicator.active {
  background-color: #26bba6;
}
</style>
