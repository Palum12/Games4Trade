<template>
    <div class="row no-gutters p-2">
        <div class="form rounded col-12 p-3">
            <form @submit.prevent="onSubmit">
                <div class="col-12">
                    <div class="form-group">
                        <label for="title">Tytuł</label>
                        <input
                                type="text"
                                id="title"
                                class="form-control"
                                v-model="title">
                    </div>
                    <div class="row">
                        <div class="col-3">
                            <p>Wybierz typ ogłoszenia: </p>
                            <div class="form-group">
                                <div class="radio">
                                    <label><input type="radio" value="Game" v-model="discriminator">Gra</label>
                                </div>
                                <div class="radio">
                                    <label><input type="radio" value="Console" v-model="discriminator">Konsola</label>
                                </div>
                                <div class="radio disabled">
                                    <label><input type="radio" value="Accessory" v-model="discriminator">Akcesorium</label>
                                </div>
                            </div>
                        </div>
                        <div class="col-4">
                            <div class="form-group">
                                <div class="input">
                                    <label for="price">Twoj wycena</label>
                                    <input type="number" class="form-control" id="price" v-model.number="price">
                                </div>
                            </div>
                            <div class="form-group">
                                <input type="checkbox" id="exchange" value="true" v-model="exchangeActive">
                                <label for="exchange">Czy chcesz się wymienić ?</label>
                            </div>
                        </div>
                    </div>
                    <div v-if="discriminator==='Accessory'" class="form-row">
                        <div class="form-group col-5">
                            <label for="manufacturer">Producent</label>
                            <input
                                    type="text"
                                    id="manufacturer"
                                    class="form-control"
                                    v-model="accessoryManufacturer">
                        </div>
                        <div class="form-group col-7">
                            <label for="manufacturer">Model</label>
                            <input
                                    type="text"
                                    id="model"
                                    class="form-control"
                                    v-model="accessoryModel">
                        </div>
                    </div>
                    <div v-if="discriminator === 'Game'" class="form-group">
                        <label for="developer">Producent</label>
                        <input
                                type="text"
                                id="developer"
                                class="form-control"
                                v-model="developer">
                    </div>
                    <div class="form-row">
                        <div class="form-group col-5">
                            <div class="input">
                                <label for="state">Stan przedmiotu</label>
                                <select class="form-control" id="state" v-model="stateId">
                                    <option v-for="state in states" :key="state.id" :value="state.id">{{state.value}}</option>
                                </select>
                            </div>
                        </div>
                        <div class="form-group col-7">
                            <div class="input">
                                <label for="system">System</label>
                                <select class="form-control" id="system" v-model="systemId">
                                    <option
                                            v-for="system in systems"
                                            :key="system.id"
                                            :value="system.id">{{system.manufacturer + ' ' + system.model}}</option>
                                </select>
                            </div>
                        </div>
                    </div>
                    <div class="form-row">
                        <div v-if="discriminator !== 'Accessory'" class="form-group col-5">
                            <div class="input">
                                <label for="region">Region</label>
                                <select class="form-control" id="region" v-model="regionId">
                                    <option
                                            v-for="region in regions"
                                            :key="region.id"
                                            :value="region.id">{{region.value}}</option>
                                </select>
                            </div>
                        </div>
                        <div v-if="discriminator === 'Game'" class="form-group col-7">
                            <div class="input">
                                <label for="genre">Gatunek</label>
                                <select class="form-control" id="genre" v-model="genreId">
                                    <option
                                            v-for="genre in genres"
                                            :key="genre.id"
                                            :value="genre.id">{{genre.value}}</option>
                                </select>
                            </div>
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="description">Opis</label>
                        <textarea
                                id="description"
                                class="form-control"
                                rows="6"
                                v-model="description">
                        </textarea>
                    </div>
                    <div class="row d-flex justify-content-around">
                        <button type="button" class="btn btn-info">Dodaj zdjęcia</button>
                        <button type="button" class="btn btn-primary">Dodaj ogłoszenie!</button>
                    </div>
                </div>
            </form>
        </div>
    </div>
</template>

<script>
import {mapGetters} from 'vuex'
export default {
  name: 'AddAdvertisement',
  props: ['userId'],
  data () {
    return {
      title: null,
      description: null,
      discriminator: 'Game',
      exchangeActive: false,
      price: null,
      stateId: null,
      systemId: null,
      regionId: null,
      genreId: null,
      developer: null,
      accessoryManufacturer: null,
      accessoryModel: null
    }
  },
  methods: {
  },
  computed: {
    ...mapGetters(['regions', 'systems', 'genres', 'states'])
  }
}
</script>

<style scoped>

</style>
