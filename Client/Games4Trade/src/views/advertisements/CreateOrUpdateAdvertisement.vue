<template>
    <div v-if="hasDataLoaded" class="row no-gutters p-2">
        <div class="form rounded col-12 p-3">
            <form @submit.prevent="onSubmit">
                <div class="col-12">
                    <div class="form-group">
                        <label for="title">Tytuł</label>
                        <input
                                type="text"
                                id="title"
                                class="form-control"
                                @blur="$v.advertisement.title.$touch()"
                                v-model="advertisement.title">
                        <p v-show="!$v.advertisement.title.required">
                            Proszę podać tytuł ogłoszenia
                        </p>
                    </div>
                    <div class="row">
                        <div class="col-3">
                            <p>Wybierz typ ogłoszenia: </p>
                            <div class="form-group noBottomMargin">
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
                            <label for="dateReleased">Data wydania przedmiotu</label>
                            <input
                                    type="date"
                                    min="1960-01-01"
                                    max="2030-01-01"
                                    class="form-control"
                                    v-bind:class="[$v.advertisement.dateReleased.$error ? invalidClass : ''
                                            , formClass]"
                                    id="dateReleased"
                                    @blur="$v.advertisement.dateReleased.$touch()"
                                    v-model="advertisement.dateReleased"
                                    >
                            <p v-show="!$v.advertisement.dateReleased.isAfter">
                                Proszę podać realną datę po roku 1960
                            </p>
                            <p v-show="!$v.advertisement.dateReleased.isBefore">
                                Proszę podać realną datę do miesiąca w przyszłość
                            </p>
                        </div>
                        <div class="col-9">
                            <div class="form-group">
                                <div class="input">
                                    <label for="price">Twoj wycena</label>
                                    <input
                                            type="number"
                                            class="form-control"
                                            v-bind:class="[$v.advertisement.price.$error ? invalidClass : ''
                                            , formClass]"
                                            id="price"
                                            @blur="$v.advertisement.price.$touch()"
                                            v-model.number="advertisement.price">
                                </div>
                                <p v-show="!$v.advertisement.price.required">
                                    Proszę podać wycenę
                                </p>
                                <p v-show="!$v.advertisement.price.minVal">
                                    Cena nie może być ujemna!
                                </p>
                            </div>
                            <div class="form-group noBottomMargin">
                                <input type="checkbox" id="exchange" v-model="advertisement.exchangeActive">
                                <label for="exchange">Czy chcesz się wymienić ?</label>
                            </div>
                            <div class="form-group noBottomMargin">
                                <input type="checkbox" id="showEmail" v-model="advertisement.showEmail">
                                <label for="showEmail">Czy chcesz się pokazać swój email w ogłoszeniu ?</label>
                            </div>
                            <div class="form-group noBottomMargin">
                                <input type="checkbox" id="showPhone" v-model="advertisement.showPhone">
                                <label for="showPhone">Czy chcesz się pokazać swój numer telefonu w ogłoszeniu (o ile go podałeś) ?</label>
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
                                    v-model="accessoryManufacturer">
                            <p v-show="!isAccessoryManufacturer">
                                Proszę podać producenta akcesorium
                            </p>
                        </div>
                        <div class="form-group col-7">
                            <label for="manufacturer">Model</label>
                            <input
                                    type="text"
                                    id="model"
                                    class="form-control"
                                    v-model="accessoryModel">
                            <p v-show="!isAccessoryModel">
                                Proszę podać model akcesorium
                            </p>
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
                                <select
                                        class="form-control"
                                        id="state"
                                        @blur="$v.advertisement.stateId.$touch()"
                                        v-model="advertisement.stateId">
                                    <option v-for="state in states" :key="state.id" :value="state.id">{{state.value}}</option>
                                </select>
                            </div>
                            <p v-show="!$v.advertisement.stateId.required">
                                Proszę wskazać stan przedmiotu ogłoszenia
                            </p>
                        </div>
                        <div class="form-group col-7">
                            <div class="input">
                                <label for="system">System</label>
                                <select
                                        class="form-control"
                                        id="system"
                                        @blur="$v.advertisement.systemId.$touch()"
                                        v-model="advertisement.systemId">
                                    <option
                                            v-for="system in systems"
                                            :key="system.id"
                                            :value="system.id">{{system.manufacturer + ' ' + system.model}}</option>
                                </select>
                            </div>
                            <p v-show="!$v.advertisement.systemId.required">
                                Proszę wybrać system
                            </p>
                        </div>
                    </div>
                    <div class="form-row">
                        <div v-if="advertisement.discriminator !== 'Accessory'" class="form-group col-5">
                            <div class="input">
                                <label for="region">Region</label>
                                <select
                                        class="form-control"
                                        id="region"
                                        v-model="regionId">
                                    <option
                                            v-for="region in regions"
                                            :key="region.id"
                                            :value="region.id">{{region.value}}</option>
                                </select>
                            </div>
                            <p v-show="!isRegionSelected">
                                Proszę wybrać region
                            </p>
                        </div>
                        <div v-if="advertisement.discriminator === 'Game'" class="form-group col-7">
                            <div class="input">
                                <label for="genre">Gatunek</label>
                                <select
                                        class="form-control"
                                        id="genre"
                                        v-model="genreId">
                                    <option
                                            v-for="genre in genres"
                                            :key="genre.id"
                                            :value="genre.id">{{genre.value}}</option>
                                </select>
                            </div>
                            <p v-show="!isGenreSelected">
                                Proszę wybrać gatunek
                            </p>
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="description">Opis</label>
                        <textarea
                                id="description"
                                class="form-control"
                                rows="3"
                                @blur="$v.advertisement.description.$touch()"
                                v-model="advertisement.description">
                        </textarea>
                        <p v-show="!$v.advertisement.description.required">
                            Opis nie może być pusty
                        </p>
                    </div>
                    <div class="row d-flex justify-content-around">
                        <button
                                type="button"
                                class="btn btn-info mx-2"
                                @click="$router.go(-1)">Powrót</button>
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
                            <button type="button" class="btn btn-danger" @click="selectedFiles = []; hasPhotoChanged= true">Usuń zdjęcia</button>
                        </div>
                        <div v-if="isEditing">
                            <button
                                    type="button"
                                    class="btn btn-warning mx-2"
                                    @click="remove">Usuń</button>
                        </div>
                        <div v-if="isEditing">
                            <button
                                    v-if="advertisement.isActive"
                                    type="button"
                                    class="btn btn-warning mx-2"
                                    @click="archive">Archiwizuj</button>
                        </div>
                        <div v-if="!isEditing">
                            <button
                                    type="button"
                                    :disabled="!isValidationOk"
                                    class="btn btn-primary"
                                    @click="saveAdd">Dodaj ogłoszenie!</button>
                        </div>
                        <div v-else>
                            <button
                                    type="button"
                                    :disabled="!isValidationOk"
                                    class="btn btn-primary"
                                    @click="saveAdd">Zapisz zmiany!</button>
                        </div>
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
import { required, minValue } from 'vuelidate/lib/validators'
export default {
  name: 'AddAdvertisement',
  data () {
    return {
      hasDataLoaded: false,
      dataSent: false,
      userId: null,
      isEditing: false,
      formClass: 'form-control',
      invalidClass: 'is-invalid',
      selectedFiles: [],
      hasPhotoChanged: false,
      advertisement: {
        id: null,
        title: null,
        dateReleased: null,
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
        accessoryModel: null,
        showEmail: false,
        showPhone: false,
        isActive: true
      },
      // due to a bug with vue
      regionId: null,
      genreId: null,
      accessoryManufacturer: null,
      accessoryModel: null
    }
  },
  watch: {
    discriminator (newVal) {
      switch (newVal) {
        case 'Game':
          this.accessoryManufacturer = null
          this.accessoryModel = null
          break
        case 'Console':
          this.accessoryManufacturer = null
          this.accessoryModel = null
          this.genreId = null
          this.advertisement.developer = null
          break
        case 'Accessory':
          this.advertisement.developer = null
          this.regionId = null
          break
        default:
          break
      }
    },
    accessoryManufacturer (newVal) {
      this.advertisement.accessoryManufacturer = newVal
    },
    accessoryModel (newVal) {
      this.advertisement.accessoryModel = newVal
    },
    genreId (newVal) {
      this.advertisement.genreId = newVal
    },
    regionId (newVal) {
      this.advertisement.regionId = newVal
    },
    '$route': function (newVal) {
      this.getData()
    }
  },
  methods: {
    getData () {
      this.hasDataLoaded = false
      this.regionId = null
      this.genreId = null
      this.accessoryManufacturer = null
      this.accessoryModel = null
      this.advertisement = {
        id: null,
        title: null,
        dateReleased: null,
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
        accessoryModel: null,
        showEmail: false,
        showPhone: false,
        isActive: true
      }
      this.selectedFiles = []
      this.isEditing = false
      let vm = this
      this.$store.dispatch('getUserId')
        .then(response => {
          vm.userId = response.data
          if (this.$route.params.id != null) {
            let id = this.$route.params.id
            axios.get(`advertisements/${id}`)
              .then(response => {
                if (response.data.userId !== vm.userId) {
                  vm.$router.push('/')
                } else {
                  vm.advertisement = response.data
                  vm.advertisement.dateReleased =
                    vm.advertisement.dateReleased == null ? null : vm.advertisement.dateReleased.substring(0, 10)
                  vm.advertisement.systemId = response.data.system.id
                  vm.advertisement.stateId = response.data.state.id
                  if (vm.advertisement.discriminator === 'Game') {
                    vm.advertisement.genreId = response.data.genre.id
                    vm.genreId = response.data.genre.id
                    vm.advertisement.regionId = response.data.region.id
                    vm.regionId = response.data.region.id
                  }
                  if (vm.advertisement.discriminator === 'Console') {
                    vm.advertisement.regionId = response.data.region.id
                    vm.regionId = response.data.region.id
                  }
                  if (vm.advertisement.discriminator === 'Accessory') {
                    vm.accessoryManufacturer = response.data.accessoryManufacturer
                    vm.accessoryModel = response.data.accessoryModel
                  }
                  if (response.data.photos.length > 0) {
                    vm.selectedFiles = response.data.photos
                  }
                  vm.isEditing = true
                  vm.hasDataLoaded = true
                }
              })
              .catch(error => {
                console.log(error)
              })
          }
          vm.hasDataLoaded = true
        })
    },
    selectedPhotos (event) {
      this.hasPhotoChanged = true
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
          if (vm.isEditing) {
            axios.put(`advertisements/${vm.advertisement.id}`, vm.advertisement)
              .then(() => {
                if (vm.hasPhotoChanged) {
                  const fd = new FormData()
                  for (let i = 0; i < vm.selectedFiles.length; i++) {
                    fd.append('', vm.selectedFiles[i], vm.selectedFiles[i].name)
                  }
                  axios.patch(`advertisements/${vm.advertisement.id}/photos`, fd,
                    {
                      headers: {
                        'Content-Type': 'multipart/form-data'
                      }
                    })
                    .then(() => {
                      vm.$store.dispatch('unsetSpinnerLoading')
                      mixins.methods.customSuccessPopUp(vm, 'Gratulacje Twoje ogłoszenie zostało zmodifkowane!')
                      vm.dataSent = true
                      vm.$router.push(`/advertisements/${vm.advertisement.id}`)
                    })
                    .catch(() => {
                      vm.$store.dispatch('unsetSpinnerLoading')
                      mixins.methods.customErrorPopUp(vm, 'Twoje ogłoszenie zostało zmodifkowane ale podczas zapisywania zdjęcia coś poszło nie tak!')
                      vm.dataSent = true
                    })
                } else {
                  vm.$store.dispatch('unsetSpinnerLoading')
                  mixins.methods.customSuccessPopUp(vm, 'Gratulacje Twoje ogłoszenie zostało zmodifkowane!')
                  vm.dataSent = true
                  vm.$router.push(`/advertisements/${vm.advertisement.id}`)
                }
              })
              .catch(() => {
                vm.$store.dispatch('unsetSpinnerLoading')
                mixins.methods.errorPopUp(vm)
              })
          } else {
            axios.post('advertisements', vm.advertisement)
              .then(response => {
                if (vm.selectedFiles.length > 0) {
                  let id = response.data
                  const fd = new FormData()
                  for (let i = 0; i < vm.selectedFiles.length; i++) {
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
                      vm.dataSent = true
                      vm.$router.push(`/advertisements/${id}`)
                    })
                    .catch(() => {
                      vm.$store.dispatch('unsetSpinnerLoading')
                      mixins.methods.customErrorPopUp(vm, 'Ups! Twoje ogłoszenie zostało dodane, ale coś poszło nie tak podczas ' +
                        'dodawania zdjęć, spróbuj je wyłączyć adblocka i dodać ponownie lub skontaktuj się z administratorem!')
                      vm.dataSent = true
                      vm.$router.push({name: 'home'})
                    })
                } else {
                  vm.$store.dispatch('unsetSpinnerLoading')
                  mixins.methods.customSuccessPopUp(vm, 'Gratulacje Twoje ogłoszenie zostało dodane i jest ono już widoczne !')
                  vm.dataSent = true
                  vm.$router.push(`/advertisements/${response.data}`)
                }
              })
              .catch(() => {
                vm.$store.dispatch('unsetSpinnerLoading')
                mixins.methods.errorPopUp(vm)
              })
          }
        })
    },
    archive () {
      let vm = this
      mixins.methods.confirmationPernamentDialog(vm)
        .then(() => {
          axios.delete(`advertisements/${this.advertisement.id}/archived`)
            .then(() => {
              vm.advertisement.isActive = false
              vm.dataSent = true
              mixins.methods.simpleSuccessPopUp(vm)
            })
            .catch(() => {
              mixins.methods.errorPopUp(vm)
            })
        })
    },
    remove () {
      let vm = this
      mixins.methods.confirmationPernamentDialog(vm)
        .then(() => {
          axios.delete(`advertisements/${this.advertisement.id}`)
            .then(() => {
              mixins.methods.simpleSuccessPopUp(vm)
              vm.dataSent = true
              vm.$router.go(-1)
            })
            .catch(() => {
              mixins.methods.errorPopUp(vm)
            })
        })
    }
  },
  computed: {
    ...mapGetters(['regions', 'systems', 'genres', 'states']),
    discriminator () {
      return this.advertisement.discriminator
    },
    // here manual validation due to vuelidate bug
    isAccessoryManufacturer () {
      return this.discriminator !== 'Accessory' || (this.accessoryManufacturer != null && this.accessoryManufacturer !== '')
    },
    isAccessoryModel () {
      return this.discriminator !== 'Accessory' || (this.accessoryModel != null && this.accessoryModel !== '')
    },
    isGenreSelected () {
      return this.discriminator !== 'Game' || this.genreId != null
    },
    isRegionSelected () {
      return this.discriminator === 'Accessory' || this.regionId != null
    },
    isValidationOk () {
      return this.isAccessoryManufacturer &&
        this.isAccessoryModel &&
        this.isGenreSelected &&
        this.isRegionSelected &&
              !this.$v.$invalid
    },
    monthFromNow () {
      let now = new Date()
      let current
      if (now.getMonth() === 11) {
        current = new Date(now.getFullYear() + 1, 0, 1)
      } else {
        current = new Date(now.getFullYear(), now.getMonth() + 1, 1)
      }
      return current
    }
  },
  validations: {
    advertisement: {
      dateReleased: {
        isAfter (date) { return date == null || date === '' || new Date(date) > new Date('1960-01-01T00:00:00Z') },
        isBefore (date) { return date == null || date === '' || new Date(date) < (this.monthFromNow) }
      },
      title: {
        required
      },
      description: {
        required
      },
      price: {
        required,
        minVal: minValue(0)
      },
      stateId: {
        required
      },
      systemId: {
        required
      }
    }
  },
  mounted () {
    this.getData()
  },
  beforeRouteEnter (to, from, next) {
    next(vm => {
      if (vm.$store.getters.isAuthenticated) {
        next()
      } else {
        next('/')
      }
    })
  },
  async beforeRouteLeave (to, from, next) {
    let vm = this
    if (!vm.isEditing && this.$route.params.id != null) {
      next()
      return
    }
    if (this.dataSent) {
      next()
      return
    }
    await mixins.methods.confirmationLeaveDialog(vm)
      .then(() => next())
      .catch(() => {
        next(false)
      }
      )
  }
}
</script>

<style scoped>
    .noBottomMargin {
        margin-bottom: 0 !important;
    }
</style>
