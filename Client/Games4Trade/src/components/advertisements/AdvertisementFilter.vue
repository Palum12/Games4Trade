<template>
        <div class="px-2 responsive-content">
            <h2>Filtruj ogłoszenia</h2>
            <div class="w-100 py-2">
                <form @submit.prevent="onSubmit" class="filter-content">
                    <div class="form-group">
                        <label for="type">Typ ogłoszenia</label>
                        <select
                                class="form-control"
                                id="type"
                                v-model="filterCriteria.type">
                            <option :value="null">Wszystkie</option>
                            <option value="game">Gra</option>
                            <option value="console">Konsola</option>
                            <option value="accessory">Akcesorium</option>
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
                        <div v-show="showSystems" v-for="system in systems" :key="'system' + system.id">
                            <input type="checkbox" :id="'system' + system.id" :value="system.id" v-model="filterCriteria.systemIds">
                            <label :for="'system' + system.id">{{system.manufacturer + ' ' + system.model}}</label>
                        </div>
                    </div>
                    <div  v-if="filterCriteria.type != null && filterCriteria.type === 'game'" class="form-group">
                        <label @click="showGenres = !showGenres">{{'Gatunek' + (showGenres ? '▲' : '▼')}}</label>
                        <div v-show="showGenres" v-for="genre in genres" :key="'genre' + genre.id">
                            <input type="checkbox" :id="'genre' + genre.id" :value="genre.id" v-model="filterCriteria.genreIds">
                            <label :for="'genre' + genre.id">{{genre.value}}</label>
                        </div>
                    </div>
                    <div v-if="filterCriteria.type != null && filterCriteria.type !== 'accessory'" class="form-group">
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
            </div >
            <div class="w-100 py-3">
                <div class="filter-buttons">
                    <button class="btn btn-warning btn-block" @click="cleanFilters">Wyczyść filtry</button>
                    <button class="btn btn-primary btn-block mb-2" @click="applyFilters">Filtruj</button>
                </div>
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
      this.$emit('clear')
    },
    applyFilters () {
      this.$emit('filter')
    }
  }
}
</script>

<style scoped>
    .labels{
        white-space:pre;
    }
    .filter-buttons {
        position: absolute;
        bottom: 0;
        left: 10px;
        right: 10px;
        height: 120px;
        padding-top: 20px;
    }
    .filter-content {
        max-height: calc(80vh - 120px);
        overflow-y: auto;
    }
    .responsive-content {
        height: 100%;
        position: relative;
    }
    @media only screen and (max-width: 768px) {
        .filter-content {
            max-height: calc(45vh - 120px);
            overflow-y: auto;
        }
        .responsive-content {
           height: 50vh;
        }
    }
</style>
