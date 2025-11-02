<template>
  <div class="spinner" role="status" aria-live="polite">
    <span class="visually-hidden">Loading...</span>
  </div>
</template>

<style scoped>
.spinner {
  width: 80px;
  height: 80px;
  border: 6px solid rgba(38, 187, 166, 0.2);
  border-top-color: #26bba6;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 0 auto;
}

.visually-hidden {
  position: absolute;
  width: 1px;
  height: 1px;
  padding: 0;
  margin: -1px;
  overflow: hidden;
  clip: rect(0, 0, 0, 0);
  white-space: nowrap;
  border: 0;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}
</style>
