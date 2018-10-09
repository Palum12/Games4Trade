<template>
    <div>
        <div class="row">
            <div class="col-3">
                <p>Twoje zdjęcie profilowe:</p>
                <img :src="`http://localhost:5000/api/users/${userId}/photo`">
            </div>
        </div>

        <div class="row mt-2 pl-3">
            <p>Twój opis</p>
        </div>
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
        border: 1px solid lightgray;
        border-radius: 5px;
    }
</style>
