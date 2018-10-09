<template>
    <div>
        <div class="form rounded m-2 p-3 systems">
            <form v-on:submit.prevent>
                <div v-if="systems!==null" v-for="system in systems" :key="system.id">
                    <div class="form-row justify-content-between text-center mb-2">
                        <div class=" col-lg-7 col-md-6 col-12">
                            <input
                                    type="text"
                                    id="manufacturer"
                                    readonly
                                    class="form-control"
                                    :value="system.manufacturer + ' ' + system.model">
                        </div>
                        <button
                                v-if="isLiked(system)"
                                class="btn btn-success ml-sm-1"
                                @click="dislike(system)">Interesuje Cię ten system!</button>
                        <button
                                v-else
                                class="btn btn-primary ml-sm-1"
                                @click="like(system)">Ten system mnie interesuje!</button>
                    </div>
                </div>
            </form>
        </div>
        <button class="btn btn-primary btn-block" @click="saveInterestingSystems">Zapisz</button>
    </div>
</template>

<script>
import mixins from '../../mixins/mixins'
import axios from 'axios'
export default {
  name: 'MySystems',
  data () {
    return {
      systems: null,
      likedSystems: null
    }
  },
  props: {
    userId: Number
  },
  methods: {
    saveInterestingSystems () {
      let vm = this
      mixins.methods.confirmationDialog(vm)
        .then(() => {
          let likedSystemsRequest = []
          vm.likedSystems.forEach(el => {
            likedSystemsRequest.push(el.id)
          })
          axios.patch(`users/${vm.userId}/systems`, likedSystemsRequest)
            .then(response => {
              mixins.methods.simpleSuccessPopUp(vm)
              vm.$forceUpdate()
            })
            .catch(error => {
              mixins.methods.errorPopUp(error.response.data)
              vm.$forceUpdate()
            })
        })
        .catch()
    },
    getSystems () {
      let vm = this
      this.$store.dispatch('getSystems')
        .then(() => {
          vm.systems = vm.$store.getters.systems
        })
    },
    getUserSystems () {
      let vm = this
      return this.$store.dispatch('getLikedSystems', this.userId)
        .then(response => {
          vm.likedSystems = response.data
        })
    },
    isLiked (system) {
      if (this.likedSystems !== null) {
        return this.likedSystems.some(x => x.id === system.id)
      }
      return false
    },
    like (system) {
      this.likedSystems.push(system)
    },
    dislike (system) {
      this.likedSystems = this.likedSystems.filter(el => el.id !== system.id)
    }
  },
  async mounted () {
    await this.getUserSystems()
    this.getSystems()
  }
}
</script>

<style scoped>
    .systems {
        min-height: 200px;
        height: 62vh;
        max-height: 90%;
        overflow: hidden;
        overflow-y: auto;
    }
    input[readonly] {
        background-color: #fff;
    }
</style>
