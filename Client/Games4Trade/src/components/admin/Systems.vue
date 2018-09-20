<template>
    <div class="form rounded m-2 p-3">
        <form v-on:submit.prevent>
            <h2 class="text-center">Systemy</h2>
            <div v-for="system in systems" :key="system.id">
                <div class="form-row text-center mb-1">
                    <div class=" col-lg-8 col-md-6 col-12">
                        <input
                                type="text"
                                id="Manufacturer"
                                class="form-control"
                                placeholder="Producent"
                                v-model="system.manufacturer"
                        >
                        <p v-if="system.manufacturer === ''">Pole nie może być puste !</p>
                        <input
                                type="text"
                                id="Model"
                                class="form-control mt-1"
                                placeholder="Model"
                                v-model="system.model"
                        >
                        <p v-if="system.model === ''">Pole nie może być puste !</p>
                    </div>
                    <div v-if="shouldSave(system.id)" class="col-lg-4 col-md-6 col-12 d-flex justify-content-end ">
                        <button class="btn btn-info"
                                :disabled="system.manufacturer === '' || system.model === ''"
                                @click="save(system)">Zapisz</button>
                    </div>
                    <div v-else class="col-lg-4 col-md-6 col-12 d-flex justify-content-end">
                        <button class="btn btn-warning"
                                :disabled="system.manufacturer === '' || system.model === ''"
                                @click="modify(system)">Modyfikuj</button>
                        <button class="btn btn-danger ml-1"
                                @click="remove(system.id)">X</button>
                    </div>
                </div>
            </div>

            <button class="btn btn-info btn-block" :disabled="!canAdd" @click="addPlace">Dodaj nowy system</button>

        </form>
    </div>
</template>

<script>
import {mapGetters} from 'vuex'
import axios from 'axios'
import mixins from '../../mixins/mixins'
export default {
  name: 'systems',
  data () {
    return {
      isDbInSync: true,
      systems: [],
      originalsystems: []
    }
  },
  computed: {
    ...mapGetters(['isSpinnerLoading']),
    canAdd () {
      return !this.systems.some(system => system.manufacturer === '' || system.model === '') && this.isDbInSync
    }
  },
  methods: {
    addPlace () {
      if (this.systems.length > 0) {
        this.systems.push({id: this.systems[this.systems.length - 1].id + 1, manufacturer: '', model: ''})
      } else {
        this.systems.push({id: 0, manufacturer: '', model: ''})
      }
      this.isDbInSync = false
    },
    save (system) {
      let vm = this
      this.$store.dispatch('setSpinnerLoading')
      mixins.methods.confirmationDialog(vm)
        .then(() => {
          axios.post('systems', {manufacturer: system.manufacturer, model: system.model})
            .then(() => {
              vm.$store.dispatch('unsetSpinnerLoading')
              mixins.methods.simpleSuccessPopUp(vm)
              vm.getsystems()
            })
            .catch(error => {
              if (error.response.status === 409) {
                mixins.methods.customErrorPopUp(vm, 'Podany system już istnieje !')
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
    modify (system) {
      let vm = this
      let copy = this.originalsystems.find((element) => element.id === system.id)
      if (system.model === copy.model && system.manufacturer === copy.manufacturer) {
        mixins.methods.customErrorPopUp(vm, 'Proszę zmień coś zanim spróbujesz zapisać zmiany !')
        return
      }
      this.$store.dispatch('setSpinnerLoading')
      mixins.methods.confirmationDialog(vm)
        .then(() => {
          axios.put(`systems/${system.id}`, {manufacturer: system.manufacturer, model: system.model})
            .then(() => {
              vm.$store.dispatch('unsetSpinnerLoading')
              mixins.methods.simpleSuccessPopUp(vm)
              vm.getsystems()
            })
            .catch(error => {
              if (error.response.status === 409) {
                mixins.methods.customErrorPopUp(vm, 'Podany system już istnieje !')
                let originalSystem = vm.originalsystems.find((element) => element.id === system.id)
                system.manufacturer = originalSystem.manufacturer
                system.model = originalSystem.model
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
    remove (systemId) {
      this.$store.dispatch('setSpinnerLoading')
      let vm = this
      mixins.methods.confirmationDialog(vm)
        .then(() => {
          axios.delete(`systems/${systemId}`)
            .then(() => {
              vm.$store.dispatch('unsetSpinnerLoading')
              mixins.methods.simpleSuccessPopUp(vm)
              vm.getsystems()
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
      if (this.systems.length > 0) {
        isLast = this.systems[this.systems.length - 1].id === id
      } else {
        isLast = true
      }
      return !this.isDbInSync && isLast
    },
    getsystems () {
      return new Promise((resolve, reject) => {
        let vm = this
        this.$store.dispatch('getSystems')
          .then(() => {
            vm.systems = vm.$store.getters.systems
            vm.originalsystems = JSON.parse(JSON.stringify(vm.systems))
            vm.isDbInSync = true
          })
          .then(() => resolve())
          .catch(error => reject(error))
      })
    }
  },
  mounted () {
    this.$store.dispatch('setSpinnerLoading')
    this.getsystems()
    this.$store.dispatch('unsetSpinnerLoading')
  }
}
</script>

<style scoped>

</style>
