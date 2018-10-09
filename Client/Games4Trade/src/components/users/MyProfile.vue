<template>
    <div>
        <p>Twoje zdjęcie profilowe:</p>
        <img :src="`http://localhost:5000/api/users/${userId}/photo`">
    </div>
</template>

<script>
export default {
  name: 'MyProfile',
  props: {
    userId: Number
  }
}
</script>

<style scoped>
    img {
        width: 150px;
        height: 150px;
        object-fit: contain;
    }
</style>
