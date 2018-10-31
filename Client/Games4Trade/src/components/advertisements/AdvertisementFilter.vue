<template>
    <div class="row">
        <h2>Filtruj ogłoszenia</h2>
        <div class="form rounded col-12">
            <form @submit.prevent="onSubmit">
                <div class="form-group">
                    <label for="type">Typ ogłoszenia</label>
                    <select
                            class="form-control"
                            id="type"
                            v-model="filterCriteria.type">
                        <option :value="null">Wszystkie</option>
                        <option value="Game">Gra</option>
                        <option value="Console">Konsola</option>
                        <option value="Accessory">Akcesorium</option>
                    </select>
                </div>
                <div class="form-group">
                    <label for="state">Stan</label>
                    <select
                            class="form-control"
                            id="state"
                            v-model="filterCriteria.stateId">
                        <option :value="null">Wszystkie</option>
                        <option v-for="state in states" :key="state.id" :value="state.id">{{state.value}}</option>
                    </select>
                </div>
                <div class="form-group">
                    <label class="labels" @click="showSystems = !showSystems">{{'System' + (showSystems ? '▲' : '▼')}}</label>
                    <div v-show="showSystems" v-for="system in systems" :key="system.id">
                        <input type="checkbox" :id="system.id" :value="system.id" v-model="filterCriteria.systemIds">
                        <label :for="system.id">{{system.manufacturer + ' ' + system.model}}</label>
                    </div>
                </div>
                <div class="form-group">
                    <label @click="showGenres = !showGenres">{{'Gatunek' + (showGenres ? '▲' : '▼')}}</label>
                    <div v-show="showGenres" v-for="genre in genres" :key="genre.id">
                        <input type="checkbox" :id="genre.id" :value="genre.id" v-model="filterCriteria.genreIds">
                        <label :for="genre.id">{{genre.value}}</label>
                    </div>
                </div>
                <div v-if="filterCriteria.type != null && filterCriteria.type !== 'Accessory'" class="form-group">
                    <label>Region</label>
                    <div v-for="region in regions" :key="region.id">
                        <input type="radio" :id="region.id" :value="region.id" v-model="filterCriteria.regionId">
                        <label :for="region.id">{{region.value}}</label>
                    </div>
                    <button
                            type="button"
                            class="btn btn-outline-info"
                            @click="filterCriteria.regionId = null">Wyczyść</button>
                </div>
            </form>
            <button class="btn btn-warning btn-block" @click="cleanFilters">Wyczyść filtry</button>
            <button class="btn btn-primary btn-block mb-2" @click="applyFilters">Filtruj</button>
        </div>
    </div>
</template>

<script>
import {mapGetters} from 'vuex'
export default {
  name: 'AdvertisementFilter',
  props: ['filterCriteria'],
  data () {
    return {
      showSystems: false,
      showGenres: false
    }
  },
  computed: {
    ...mapGetters(['regions', 'systems', 'genres', 'states'])
  },
  methods: {
    cleanFilters () {
      console.log('filter')
    },
    applyFilters () {
      console.log('test')
    }
  }
}
</script>

<style scoped>
    .labels{
        white-space:pre;
    }
</style>
