<template>
    <div class="form rounded m-2 p-3">
        <form v-on:submit.prevent>
            <h2 class="text-center">Gatunki</h2>
            <div v-for="genre in genres" :key="genre.id">
                <div class="form-row text-center mb-1">
                    <div class=" col-lg-8 col-md-6 col-12">
                        <input
                                type="text"
                                id="key"
                                class="form-control"
                                v-model="genre.value"
                        >
                        <p v-if="genre.value === ''">Pole nie może być puste !</p>
                    </div>
                    <div v-if="shouldSave(genre.id)" class="col-lg-4 col-md-6 col-12 d-flex justify-content-end">
                        <button class="btn btn-info" :disabled="genre.value === ''" @click="save(genre)">Zapisz</button>
                    </div>
                    <div v-else class="col-lg-4 col-md-6 col-12 d-flex justify-content-end">
                        <button class="btn btn-warning"
                                :disabled="genre.value === ''"
                                @click="modify(genre)">Modyfikuj</button>
                        <button class="btn btn-danger ml-1"
                                @click="remove(genre.id)">X</button>
                    </div>
                </div>
            </div>

            <button class="btn btn-info btn-block" :disabled="!canAdd" @click="addPlace">Dodaj nowy gatunek</button>

        </form>
    </div>
</template>

<script>
import {mapGetters} from 'vuex'
import axios from 'axios'
import mixins from '../../mixins/mixins'
export default {
  name: 'Genres',
  data () {
    return {
      isDbInSync: true,
      genres: [],
      originalGenres: []
    }
  },
  computed: {
    ...mapGetters(['isSpinnerLoading']),
    canAdd () {
      return !this.genres.some(genre => genre.value === '') && this.isDbInSync
    }
  },
  methods: {
    addPlace () {
      if (this.genres.length > 0) {
        this.genres.push({id: this.genres[this.genres.length - 1].id + 1, value: ''})
      } else {
        this.genres.push({id: 0, value: ''})
      }
      this.isDbInSync = false
    },
    save (genre) {
      let vm = this
      this.$store.dispatch('setSpinnerLoading')
      mixins.methods.confirmationDialog(vm)
        .then(() => {
          axios.post('genres', {value: genre.value})
            .then(() => {
              vm.$store.dispatch('unsetSpinnerLoading')
              mixins.methods.simpleSuccessPopUp(vm)
              vm.getGenres()
            })
            .catch(error => {
              if (error.response.status === 409) {
                mixins.methods.customErrorPopUp(vm, 'Podany gatunek już istnieje !')
              } else {
                mixins.methods.errorPopUp(vm)
              }
              vm.$store.dispatch('unsetSpinnerLoading')
            })
        })
        .catch(() => {
          vm.$store.dispatch('unsetSpinnerLoading')
        })
    },
    modify (genre) {
      let vm = this
      if (genre.value === this.originalGenres.find((element) => element.id === genre.id).value) {
        mixins.methods.customErrorPopUp(vm, 'Proszę zmień coś zanim spróbujesz zapisać zmiany !')
        return
      }
      this.$store.dispatch('setSpinnerLoading')
      mixins.methods.confirmationDialog(vm)
        .then(() => {
          axios.put(`genres/${genre.id}`, {value: genre.value})
            .then(() => {
              vm.$store.dispatch('unsetSpinnerLoading')
              mixins.methods.simpleSuccessPopUp(vm)
              vm.getGenres()
            })
            .catch(error => {
              if (error.response.status === 409) {
                mixins.methods.customErrorPopUp(vm, 'Podany gatunek już istnieje !')
                genre.value = vm.originalGenres.find((element) => element.id === genre.id).value
              } else {
                mixins.methods.errorPopUp(vm)
              }
              vm.$store.dispatch('unsetSpinnerLoading')
            })
        })
        .catch(() => {
          vm.$store.dispatch('unsetSpinnerLoading')
        })
    },
    remove (genreId) {
      this.$store.dispatch('setSpinnerLoading')
      let vm = this
      mixins.methods.confirmationDialog(vm)
        .then(() => {
          axios.delete(`genres/${genreId}`)
            .then(() => {
              vm.$store.dispatch('unsetSpinnerLoading')
              mixins.methods.simpleSuccessPopUp(vm)
              vm.getGenres()
            })
            .catch(() => {
              vm.$store.dispatch('unsetSpinnerLoading')
              mixins.methods.errorPopUp(vm)
            })
        })
        .catch(() => {
          vm.$store.dispatch('unsetSpinnerLoading')
        })
    },
    shouldSave (id) {
      let isLast
      if (this.genres.length > 0) {
        isLast = this.genres[this.genres.length - 1].id === id
      } else {
        isLast = true
      }
      return !this.isDbInSync && isLast
    },
    getGenres () {
      return new Promise((resolve, reject) => {
        let vm = this
        this.$store.dispatch('getGenres')
          .then(() => {
            vm.genres = vm.$store.getters.genres
            vm.originalGenres = JSON.parse(JSON.stringify(vm.genres))
            vm.isDbInSync = true
          })
          .then(() => resolve())
          .catch(error => reject(error))
      })
    }
  },
  mounted () {
    this.$store.dispatch('setSpinnerLoading')
    this.getGenres()
    this.$store.dispatch('unsetSpinnerLoading')
  }
}
</script>

<style scoped>

</style>
