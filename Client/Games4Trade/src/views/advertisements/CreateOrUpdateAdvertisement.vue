<template>
    <div v-if="hasDataLoaded" class="container-xl py-3">
        <div class="form rounded-3 shadow-sm p-4">
            <form novalidate @submit.prevent="saveAdd">
                <div class="row g-3">
                    <div class="col-12">
                        <p class="form-text mb-0">Pola oznaczone <span class="text-danger">*</span> są wymagane.</p>
                    </div>
                    <div class="form-group">
                        <label for="title">Tytuł <span class="text-danger">*</span></label>
                        <input
                                type="text"
                                id="title"
                                class="form-control"
                                :class="{ 'is-invalid': v$.advertisement.title.$error }"
                                @blur="v$.advertisement.title.$touch()"
                                v-model="advertisement.title">
                        <div v-if="v$.advertisement.title.$error" class="invalid-feedback">
                            Proszę podać tytuł ogłoszenia
                        </div>
                    </div>
                    <div class="row g-3">
                        <div class="col-12 col-md-3">
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
                            <label for="dateReleased">Data wydania przedmiotu <span class="text-danger">*</span></label>
                            <input
                                    type="date"
                                    min="1960-01-01"
                                    max="2030-01-01"
                                    class="form-control"
                                    :class="{ 'is-invalid': v$.advertisement.dateReleased.$error }"
                                    id="dateReleased"
                                    @blur="v$.advertisement.dateReleased.$touch()"
                                    v-model="advertisement.dateReleased"
                                    >
                            <div v-if="v$.advertisement.dateReleased.$error" class="invalid-feedback">
                                Proszę podać poprawną datę wydania po 1960 roku.
                            </div>
                        </div>
                        <div class="col-12 col-md-9">
                            <div class="form-group">
                                <div class="input">
                                    <label for="price">Twoja wycena <span class="text-danger">*</span></label>
                                    <input
                                            type="text"
                                            class="form-control"
                                            :class="{ 'is-invalid': v$.advertisement.price.$error }"
                                            id="price"
                                            @blur="v$.advertisement.price.$touch()"
                                            v-model.number="advertisement.price">
                                </div>
                                <div v-if="v$.advertisement.price.$error" class="invalid-feedback">
                                    Proszę podać dodatnią wycenę liczbową.
                                </div>
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
                    <div v-if="advertisement.discriminator==='Accessory'" class="row g-3">
                        <div class="form-group col-12 col-md-5">
                            <label for="manufacturer">Producent <span class="text-danger">*</span></label>
                            <input
                                    type="text"
                                    id="manufacturer"
                                    class="form-control"
                                    :class="{ 'is-invalid': hasAttemptedSubmit && !isAccessoryManufacturer }"
                                    v-model="accessoryManufacturer">
                            <div v-if="hasAttemptedSubmit && !isAccessoryManufacturer" class="invalid-feedback">
                                Proszę podać producenta akcesorium
                            </div>
                        </div>
                        <div class="form-group col-12 col-md-7">
                            <label for="model">Model <span class="text-danger">*</span></label>
                            <input
                                    type="text"
                                    id="model"
                                    class="form-control"
                                    :class="{ 'is-invalid': hasAttemptedSubmit && !isAccessoryModel }"
                                    v-model="accessoryModel">
                            <div v-if="hasAttemptedSubmit && !isAccessoryModel" class="invalid-feedback">
                                Proszę podać model akcesorium
                            </div>
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
                    <div class="row g-3">
                        <div class="form-group col-12 col-md-5">
                            <div class="input">
                                <label for="state">Stan przedmiotu <span class="text-danger">*</span></label>
                                <select
                                        class="form-control"
                                        :class="{ 'is-invalid': v$.advertisement.stateId.$error }"
                                        id="state"
                                        @blur="v$.advertisement.stateId.$touch()"
                                        v-model="advertisement.stateId">
                                    <option v-for="state in states" :key="state.id" :value="state.id">{{state.value}}</option>
                                </select>
                            </div>
                            <div v-if="v$.advertisement.stateId.$error" class="invalid-feedback">
                                Proszę wskazać stan przedmiotu ogłoszenia
                            </div>
                        </div>
                        <div class="form-group col-12 col-md-7">
                            <div class="input">
                                <label for="system">System <span class="text-danger">*</span></label>
                                <select
                                        class="form-control"
                                        :class="{ 'is-invalid': v$.advertisement.systemId.$error }"
                                        id="system"
                                        @blur="v$.advertisement.systemId.$touch()"
                                        v-model="advertisement.systemId">
                                    <option
                                            v-for="system in systems"
                                            :key="system.id"
                                            :value="system.id">{{system.manufacturer + ' ' + system.model}}</option>
                                </select>
                            </div>
                            <div v-if="v$.advertisement.systemId.$error" class="invalid-feedback">
                                Proszę wybrać system
                            </div>
                        </div>
                    </div>
                    <div class="row g-3">
                        <div v-if="advertisement.discriminator !== 'Accessory'" class="form-group col-12 col-md-5">
                            <div class="input">
                                <label for="region">Region <span class="text-danger">*</span></label>
                                <select
                                        class="form-control"
                                        :class="{ 'is-invalid': hasAttemptedSubmit && !isRegionSelected }"
                                        id="region"
                                        v-model="regionId">
                                    <option
                                            v-for="region in regions"
                                            :key="region.id"
                                            :value="region.id">{{region.value}}</option>
                                </select>
                            </div>
                            <div v-if="hasAttemptedSubmit && !isRegionSelected" class="invalid-feedback">
                                Proszę wybrać region
                            </div>
                        </div>
                        <div v-if="advertisement.discriminator === 'Game'" class="form-group col-12 col-md-7">
                            <div class="input">
                                <label for="genre">Gatunek <span class="text-danger">*</span></label>
                                <select
                                        class="form-control"
                                        :class="{ 'is-invalid': hasAttemptedSubmit && !isGenreSelected }"
                                        id="genre"
                                        v-model="genreId">
                                    <option
                                            v-for="genre in genres"
                                            :key="genre.id"
                                            :value="genre.id">{{genre.value}}</option>
                                </select>
                            </div>
                            <div v-if="hasAttemptedSubmit && !isGenreSelected" class="invalid-feedback">
                                Proszę wybrać gatunek
                            </div>
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="description">Opis <span class="text-danger">*</span></label>
                        <textarea
                                id="description"
                                class="form-control"
                                :class="{ 'is-invalid': v$.advertisement.description.$error }"
                                rows="3"
                                @blur="v$.advertisement.description.$touch()"
                                v-model="advertisement.description">
                        </textarea>
                        <div v-if="v$.advertisement.description.$error" class="invalid-feedback">
                            Opis nie może być pusty
                        </div>
                    </div>
                    <div v-if="photoPreviews.length > 0" class="photo-preview-section">
                        <p class="mb-2">Podgląd zdjęć ({{ photoPreviews.length }})</p>
                        <div class="photo-preview-grid">
                            <figure v-for="preview in photoPreviews" :key="preview.key" class="photo-preview">
                                <img
                                        :src="preview.url"
                                        :alt="`Podgląd zdjęcia: ${preview.name}`"
                                        :title="preview.name"
                                        data-testid="photo-preview">
                                <figcaption>{{ preview.name }}</figcaption>
                            </figure>
                        </div>
                    </div>
                    <div class="d-flex flex-wrap gap-2 border-top pt-3 mt-2">
                        <button
                                type="button"
                                class="btn btn-outline-secondary"
                                @click="$router.go(-1)">Powrót</button>
                        <div v-if="selectedFiles.length === 0">
                            <input
                                    type="file"
                                    style="display: none"
                                    ref="fileInput"
                                    accept="image/x-png, image/jpeg"
                                    multiple="multiple"
                                    @change="selectedPhotos">
                            <button type="button" class="btn btn-info" @click="$refs.fileInput.click()" title="Uwaga, można dodać tylko zdjęcia poniżej 3 MB!">Dodaj zdjęcia</button>
                        </div>
                        <div v-else>
                            <button type="button" class="btn btn-danger" @click="removeSelectedPhotos">Usuń zdjęcia</button>
                        </div>
                        <div v-if="isEditing">
                            <button
                                    type="button"
                                    class="btn btn-warning"
                                    @click="remove">Usuń</button>
                        </div>
                        <div v-if="isEditing">
                            <button
                                    v-if="advertisement.isActive"
                                    type="button"
                                    class="btn btn-warning"
                                    @click="archive">Archiwizuj</button>
                        </div>
                        <div v-if="!isEditing" >
                            <button
                                    type="button"
                                    class="btn btn-primary"
                                    @click="saveAdd">Dodaj ogłoszenie!</button>
                        </div>
                        <div v-else>
                            <button
                                    type="button"
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
import { mapGetters } from 'vuex'
import mixins from '../../mixins/mixins'
import axios from 'axios'
import useVuelidate from '@vuelidate/core'
import { required, minValue, decimal } from '@vuelidate/validators'
export default {
  name: 'AddAdvertisement',
  setup () {
    return { v$: useVuelidate() }
  },
  data () {
    return {
      hasDataLoaded: false,
      dataSent: false,
      userId: null,
      isEditing: false,
      hasAttemptedSubmit: false,
      selectedFiles: [],
      photoPreviews: [],
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
      this.setPhotoPreviews([])
      this.selectedFiles = []
      this.isEditing = false
      this.hasAttemptedSubmit = false
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
                    vm.setPhotoPreviews(response.data.photos)
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
      const files = Array.from(event.target.files)
      this.$store.dispatch('setSpinnerLoading')
      for (var i = 0; i < files.length; i++) {
        let file = files[i]
        if (!file.type.includes('image')) {
          this.$store.dispatch('unsetSpinnerLoading')
          mixins.methods.customErrorPopUp(this, 'Wybrane rozszerzenie pliku nie jest wspierane!')
          event.target.value = ''
          return
        }
        let fileSize = file.size / 1024 / 1024
        if (fileSize > 3) {
          this.$store.dispatch('unsetSpinnerLoading')
          mixins.methods.customErrorPopUp(this, 'Wybrany plik jest większy niż 3 MB!')
          event.target.value = ''
          return
        }
      }
      this.hasPhotoChanged = true
      this.selectedFiles = files
      this.setPhotoPreviews(files)
      this.$store.dispatch('unsetSpinnerLoading')
    },
    setPhotoPreviews (photos) {
      this.clearPhotoPreviews()
      const baseUrl = axios.defaults.baseURL ? axios.defaults.baseURL.replace(/\/$/, '') : ''
      this.photoPreviews = Array.from(photos).map((photo, index) => {
        if (photo instanceof File) {
          return {
            key: `${photo.name}-${photo.lastModified}-${index}`,
            name: photo.name,
            url: URL.createObjectURL(photo),
            isObjectUrl: true
          }
        }
        return {
          key: `saved-${photo.id}`,
          name: `Zdjęcie ${index + 1}`,
          url: `${baseUrl}/advertisements/${this.advertisement.id}/photos/${photo.id}`,
          isObjectUrl: false
        }
      })
    },
    clearPhotoPreviews () {
      this.photoPreviews.forEach(preview => {
        if (preview.isObjectUrl) {
          URL.revokeObjectURL(preview.url)
        }
      })
      this.photoPreviews = []
    },
    removeSelectedPhotos () {
      this.selectedFiles = []
      this.hasPhotoChanged = true
      this.clearPhotoPreviews()
    },
    saveAdd () {
      this.hasAttemptedSubmit = true
      this.v$.$touch()
      if (!this.isValidationOk) {
        return
      }

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
                    fd.append('photos', vm.selectedFiles[i], vm.selectedFiles[i].name)
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
                    fd.append('photos', vm.selectedFiles[i], vm.selectedFiles[i].name)
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
                        'dodawania zdjęć, skontaktuj się z administratorem!')
                      vm.dataSent = true
                      vm.$router.push({name: 'home'})
                    })
                } else {
                  vm.$store.dispatch('unsetSpinnerLoading')
                  vm.dataSent = true
                  mixins.methods
                    .customSuccessPopUp(vm, 'Gratulacje Twoje ogłoszenie zostało dodane ' +
                      'i jest ono już widoczne !')
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
              !this.v$.$invalid
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
  validations () {
    return {
      advertisement: {
        dateReleased: {
          required,
          isAfter (date) {
            return date == null || date === '' ||
            new Date(date) > new Date('1960-01-01T00:00:00Z')
          },
          isBefore: (date) => {
            return date == null || date === '' ||
              new Date(date) < (this.monthFromNow)
          }
        },
        title: {
          required
        },
        description: {
          required
        },
        price: {
          required,
          decimal,
          minVal: minValue(0)
        },
        stateId: {
          required
        },
        systemId: {
          required
        }
      }
    }
  },
  mounted () {
    this.getData()
  },
  beforeUnmount () {
    this.clearPhotoPreviews()
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
    .form-group {
        margin-bottom: 0;
    }
    .invalid-feedback {
        display: block;
    }
    .noBottomMargin {
        margin-bottom: 0 !important;
    }
    .photo-preview-section {
        border-top: 1px solid var(--bs-border-color, #dee2e6);
        margin-top: 0.5rem;
        padding-top: 1rem;
    }
    .photo-preview-grid {
        display: flex;
        flex-wrap: wrap;
        gap: 0.75rem;
    }
    .photo-preview {
        margin: 0;
        width: 104px;
    }
    .photo-preview img {
        aspect-ratio: 1;
        border: 1px solid var(--bs-border-color, #dee2e6);
        border-radius: 0.375rem;
        display: block;
        object-fit: cover;
        width: 100%;
    }
    .photo-preview figcaption {
        font-size: 0.75rem;
        margin-top: 0.25rem;
        overflow: hidden;
        text-overflow: ellipsis;
        white-space: nowrap;
    }
</style>
