<template>
    <div>
        <router-link :to="'/advertisements/'+advertisement.id" tag="span">
            <a class="list-group-item list-group-item-action flex-column align-items-start mb-1">
                <div class="row">
                    <div class="col-2">
                        <img :src="`http://localhost:5000/api/advertisements/${advertisement.id}/photos/default`">
                    </div>
                    <div class="col-9">
                            <div class="row">
                                <div class="col-10">
                                    <div class="row">
                                        <h5 class="mb-1">{{advertisement.title.substring(0,80)}}</h5>
                                    </div>
                                    <div class="row">
                                        <p class="infos" v-if="advertisement.exchangeActive">Możliwa wymiana</p>
                                    </div>
                                    <div class="row">
                                        <p>Cena: {{advertisement.price}}zł</p>
                                    </div>
                                </div>
                                <div class="col-2">
                                    <small>{{advertisement.dateCreated.substring(0,10)
                                        + ' ' + advertisement.dateCreated.substring(11,16)}}</small>
                                </div>
                            </div>
                    </div>
                </div>
            </a>
        </router-link>
    </div>
</template>

<script>
export default {
  name: 'AdvertisementHeader',
  props: ['advertisement']
}
</script>

<style scoped>
    img {
        width: 9vw;
        height: 7vw;
        object-fit: cover;
        border: 1px solid lightgray;
        border-radius: 5px;
    }
    .infos {
        margin-bottom: 0 !important;
    }
</style>
