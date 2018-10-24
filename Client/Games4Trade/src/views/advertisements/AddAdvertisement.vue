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
                                v-model="advertisement.title">
                    </div>
                    <div class="row">
                        <div class="col-3">
                            <p>Wybierz typ ogłoszenia: </p>
                            <div class="form-group">
                                <div class="radio">
                                    <label><input type="radio" value="Game" v-model="advertisement.discriminator">Gra</label>
                                </div>
                                <div class="radio">
                                    <label><input type="radio" value="Console" v-model="advertisement.discriminator">Konsola</label>
                                </div>
                                <div class="radio disabled">
                                    <label><input type="radio" value="Accessory" v-model="advertisement.discriminator">Akcesorium</label>
                                </div>
                            </div>
                        </div>
                        <div class="col-4">
                            <div class="form-group">
                                <div class="input">
                                    <label for="price">Twoj wycena</label>
                                    <input type="number" class="form-control" id="price" v-model.number="advertisement.price">
                                </div>
                            </div>
                            <div class="form-group">
                                <input type="checkbox" id="exchange" value="true" v-model="advertisement.exchangeActive">
                                <label for="exchange">Czy chcesz się wymienić ?</label>
                            </div>
                        </div>
                    </div>
                    <div v-if="advertisement.discriminator==='Accessory'" class="form-row">
                        <div class="form-group col-5">
                            <label for="manufacturer">Producent</label>
                            <input
                                    type="text"
                                    id="manufacturer"
                                    class="form-control"
                                    v-model="advertisement.accessoryManufacturer">
                        </div>
                        <div class="form-group col-7">
                            <label for="manufacturer">Model</label>
                            <input
                                    type="text"
                                    id="model"
                                    class="form-control"
                                    v-model="advertisement.accessoryModel">
                        </div>
                    </div>
                    <div v-if="advertisement.discriminator === 'Game'" class="form-group">
                        <label for="developer">Producent</label>
                        <input
                                type="text"
                                id="developer"
                                class="form-control"
                                v-model="advertisement.developer">
                    </div>
                    <div class="form-row">
                        <div class="form-group col-5">
                            <div class="input">
                                <label for="state">Stan przedmiotu</label>
                                <select class="form-control" id="state" v-model="advertisement.stateId">
                                    <option v-for="state in states" :key="state.id" :value="state.id">{{state.value}}</option>
                                </select>
                            </div>
                        </div>
                        <div class="form-group col-7">
                            <div class="input">
                                <label for="system">System</label>
                                <select class="form-control" id="system" v-model="advertisement.systemId">
                                    <option
                                            v-for="system in systems"
                                            :key="system.id"
                                            :value="system.id">{{system.manufacturer + ' ' + system.model}}</option>
                                </select>
                            </div>
                        </div>
                    </div>
                    <div class="form-row">
                        <div v-if="advertisement.discriminator !== 'Accessory'" class="form-group col-5">
                            <div class="input">
                                <label for="region">Region</label>
                                <select class="form-control" id="region" v-model="advertisement.regionId">
                                    <option
                                            v-for="region in regions"
                                            :key="region.id"
                                            :value="region.id">{{region.value}}</option>
                                </select>
                            </div>
                        </div>
                        <div v-if="advertisement.discriminator === 'Game'" class="form-group col-7">
                            <div class="input">
                                <label for="genre">Gatunek</label>
                                <select class="form-control" id="genre" v-model="advertisement.genreId">
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
                                v-model="advertisement.description">
                        </textarea>
                    </div>
                    <div class="row d-flex justify-content-around">
                        <div v-if="selectedFiles.length === 0">
                            <input
                                    type="file"
                                    style="display: none"
                                    ref="fileInput"
                                    accept="image/x-png, image/jpeg"
                                    multiple="multiple"
                                    @change="selectedPhotos">
                            <button type="button" class="btn btn-info" @click="$refs.fileInput.click()">Dodaj zdjęcia</button>
                            <small class=" ml-2 mt-2 font-italic font-weight-light">Uwaga, można dodać tylko zdjęcia poniżej 3 MB!</small>
                        </div>
                        <div v-else>
                            <button type="button" class="btn btn-danger" @click="selectedFiles = []">Usuń zdjęcia</button>
                        </div>
                        <button type="button" class="btn btn-primary" @click="saveAdd">Dodaj ogłoszenie!</button>
                    </div>
                </div>
            </form>
        </div>
    </div>
</template>

<script>
import {mapGetters} from 'vuex'
import mixins from '../../mixins/mixins'
import axios from 'axios'
export default {
  name: 'AddAdvertisement',
  data () {
    return {
      userId: null,
      advertisement: {
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
      },
      selectedFiles: []
    }
  },
  watch: {
    discriminator (newVal) {
      switch (newVal) {
        case 'Game':
          this.advertisement.accessoryManufacturer = null
          this.advertisement.accessoryModel = null
          break
        case 'Console':
          this.advertisement.accessoryManufacturer = null
          this.advertisement.accessoryModel = null
          this.advertisement.genreId = null
          this.advertisement.developer = null
          break
        case 'Accessory':
          this.advertisement.genreId = null
          this.advertisement.developer = null
          this.advertisement.regionId = null
          break
        default:
          break
      }
    }
  },
  methods: {
    selectedPhotos (event) {
      this.$store.dispatch('setSpinnerLoading')
      this.selectedFiles = event.target.files
      for (var i = 0; i < this.selectedFiles.length; i++) {
        let file = this.selectedFiles[i]
        if (!file.type.includes('image')) {
          this.$store.dispatch('unsetSpinnerLoading')
          mixins.methods.customErrorPopUp(this, 'Wybrane rozszerzenie pliku nie jest wspierane!')
          this.selectedFiles = []
          return
        }
        let fileSize = file.size / 1024 / 1024
        if (fileSize > 3) {
          this.$store.dispatch('unsetSpinnerLoading')
          mixins.methods.customErrorPopUp(this, 'Wybrany plik jest większy niż 3 MB!')
          this.selectedFiles = []
          return
        }
      }
      this.$store.dispatch('unsetSpinnerLoading')
    },
    saveAdd () {
      let vm = this
      mixins.methods.confirmationDialog(vm)
        .then(() => {
          vm.$store.dispatch('setSpinnerLoading')
          axios.post('advertisements', vm.advertisement)
            .then(response => {
              if (vm.selectedFiles.length > 0) {
                let id = response.data
                const fd = new FormData()
                for (var i = 0; i < vm.selectedFiles.length; i++) {
                  fd.append('', vm.selectedFiles[i], vm.selectedFiles[i].name)
                }
                axios.patch(`advertisements/${id}/photos`, fd,
                  {
                    headers: {
                      'Content-Type': 'multipart/form-data'
                    }
                  })
                  .then(() => {
                    vm.$store.dispatch('unsetSpinnerLoading')
                    mixins.methods.customSuccessPopUp(vm, 'Gratulacje Twoje ogłoszenie zostało dodane i jest ono już widoczne !')
                    vm.$router.push({name: 'home'})
                  })
                  .catch(() => {
                    vm.$store.dispatch('unsetSpinnerLoading')
                    mixins.methods.customErrorPopUp(vm, 'Ups! Twoje ogłoszenie zostało dodane, ale coś poszło nie tak podczas ' +
                      'dodawania zdjęć, spróbuj je wyłączyć adblocka i dodać ponownie lub skontaktuj się z administratorem!')
                    vm.$router.push({name: 'home'})
                  })
              } else {
                vm.$store.dispatch('unsetSpinnerLoading')
                mixins.methods.customSuccessPopUp(vm, 'Gratulacje Twoje ogłoszenie zostało dodane i jest ono już widoczne !')
                vm.$router.push({name: 'home'})
              }
            })
            .catch(() => {
              vm.$store.dispatch('unsetSpinnerLoading')
              mixins.methods.errorPopUp(vm)
            })
        })
    }
  },
  computed: {
    ...mapGetters(['regions', 'systems', 'genres', 'states']),
    discriminator () {
      return this.advertisement.discriminator
    }
  },
  mounted () {
    var vm = this
    this.$store.dispatch('getUserId')
      .then(response => {
        vm.userId = response.data
      })
  },
  beforeRouteEnter (to, from, next) {
    next(vm => {
      if (vm.$store.getters.isAuthenticated) {
        next()
      } else {
        next('/')
      }
    })
  }
}
</script>

<style scoped>

</style>
