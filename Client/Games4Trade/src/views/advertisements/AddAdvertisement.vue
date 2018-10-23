<template>
    <div class="row no-gutters p-2">
        <div class="form rounded col-12 p-3">
            <form @submit.prevent="onSubmit">
                <div class="col-9">
                    <div class="form-group">
                        <label for="title">Tytuł</label>
                        <input
                                type="text"
                                id="title"
                                class="form-control"
                                v-model="title">
                    </div>
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
                    <div class="form-group">
                        <div class="input">
                            <label for="state">Stan przedmiotu</label>
                            <select class="form-control ml-2" id="state" v-model="stateId">
                                <option v-for="state in states" :key="state.id" :value="state.id">{{state.value}}</option>
                            </select>
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="input">
                            <label for="system">System</label>
                            <select class="form-control ml-2" id="system" v-model="systemId">
                                <option
                                        v-for="system in systems"
                                        :key="system.id"
                                        :value="system.id">{{system.manufacturer + ' ' + system.model}}</option>
                            </select>
                        </div>
                    </div>
                    <div v-if="discriminator !== 'Accessory'" class="form-group">
                        <div class="input">
                            <label for="region">Region</label>
                            <select class="form-control ml-2" id="region" v-model="regionId">
                                <option
                                        v-for="region in regions"
                                        :key="region.id"
                                        :value="region.id">{{region.value}}</option>
                            </select>
                        </div>
                    </div>
                    <div v-if="discriminator === 'Game'" class="form-group">
                        <div class="input">
                            <label for="genre">Gatunek</label>
                            <select class="form-control ml-2" id="genre" v-model="genreId">
                                <option
                                        v-for="genre in genres"
                                        :key="genre.id"
                                        :value="genre.id">{{genre.value}}</option>
                            </select>
                        </div>
                    </div>
                </div>
                <div class="col-3">
                </div>
                <!--<div class="input">
                    <label for="type">Typ</label>
                    <select class="form-control ml-2" id="type" v-model="discriminator">
                        <option value="Game">Gra</option>
                        <option value="Accessory">Akcesorium</option>
                        <option value="Console">Konsola</option>
                    </select>
                </div>-->
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
      title: '',
      description: '',
      discriminator: 'Game',
      exchangeActive: false,
      price: null,
      stateId: null,
      systemId: null,
      regionId: null,
      genreId: null,
      developer: String,
      accessoryManufacturer: String,
      accessoryModel: String
    }
  },
  methods: {
    convertDiscriminatorToEng () {
      switch (this.discriminator) {
        case 'Gra':
          return 'Game'
        case 'Konsola':
          return 'Console'
        case 'Akcesorium':
          return 'Accessory'
        default:
          break
      }
    },
    test () {
      console.log(this.regions)
    }
  },
  computed: {
    ...mapGetters(['regions', 'systems', 'genres', 'states'])
  }
}
</script>

<style scoped>

</style>
