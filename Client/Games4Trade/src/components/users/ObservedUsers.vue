<template>
    <div id="inner">
        <div class="list-group" v-for="user in users" :key="user.id">
                <div class="list-group-item list-group-item-action mb-2">
                    <div class="row">
                        <div class="col-1">
                            <router-link :to="'/users/'+user.id" tag="div" class="userLink">
                            <div class="row">
                                <img :src="getPhotoUrl(user.id)">
                            </div>
                            <div class="row">
                                <p class="mb-1 font-weight-bold">{{user.login}}</p>
                            </div>
                            </router-link>
                        </div>
                        <div class="col-9">
                            <div class="row">
                                {{prepareDescription(user.description)}}
                            </div>
                            <div class="row mt-2">
                                {{prepareGenres(user.likedGenres)}}
                            </div>
                            <div class="row">
                                {{prepareSystems(user.interestingSystems)}}
                            </div>
                        </div>
                        <div class="col-2 d-flex w-100 justify-content-end">
                            <button class="btn btn-danger mt-1 mb-2 mx-2" @click="deleteObservedUser(user.id)">Przestań<br>obserwować</button>
                        </div>
                    </div>
                </div>
        </div>
    </div>
</template>

<script>
import mixins from '../../mixins/mixins'
import axios from 'axios'
export default {
  name: 'ObservedUsers',
  props: {
    userId: Number
  },
  data () {
    return {
      users: [],
      nextPage: 2,
      isNextPage: true,
      noDescriptionMessage: 'Wygląda na to, że ten użytkonik nie posiada jeszcze opisu!',
      noGenresMessage: 'Ten użytkonik nie polubił żadnych gatunków!',
      noSystemsMessage: 'Tego użytkownika nie interesują żadne systemy!'
    }
  },
  methods: {
    async getObservedUsers () {
      let vm = this
      await axios.get(`users/${this.userId}/observed?page=1`)
        .then(response => {
          vm.users = response.data
        })
        .catch(error => {
          console.log(error)
        })
    },
    getPhotoUrl (userId) {
      return process.env.VUE_APP_API_URL + `users/${userId}/photo`
    },
    prepareDescription (content) {
      if (content == null) {
        return this.noDescriptionMessage
      }
      if (content.length > 97) {
        return content.substring(0, 97) + '...'
      }
      return content
    },
    prepareGenres (genres) {
      if (genres.length === 0) {
        return this.noGenresMessage
      }
      let result = 'Gatunki: '
      let endOfLoop = false
      for (let i = 0; i < genres.length && !endOfLoop; i++) {
        if (result.length > 100) {
          result = result + ' i więcej.....'
          endOfLoop = true
        } else {
          result = result + genres[i] + ', '
        }
      }
      result = result.slice(0, -2)
      return result
    },
    prepareSystems (systems) {
      if (systems.length === 0) {
        return this.noGenresMessage
      }
      let result = 'Systemy: '
      let endOfLoop = false
      for (let i = 0; i < systems.length && !endOfLoop; i++) {
        if (result.length > 100) {
          result = result.slice(0, -2)
          result = result + ' i więcej.....'
          endOfLoop = true
        } else {
          result = result + systems[i] + ', '
        }
      }
      result = result.slice(0, -2)
      return result
    },
    getNextPageUsers () {
      let vm = this
      axios.get(`/users/${this.userId}/observed/?page=${this.nextPage}`)
        .then(response => {
          vm.users.push(...response.data)
          let data = response.data
          if (data.length === 0) {
            vm.isNextPage = false
          } else {
            vm.nextPage = vm.nextPage + 1
          }
        })
    },
    deleteObservedUser (id) {
      let vm = this
      mixins.methods.confirmationDialog(vm)
        .then(() => {
          axios.delete(`/users/${vm.userId}/observed/`, { data: {
            ObservingUserId: vm.userId,
            ObservedUserId: id
          }
          })
            .then(() => {
              mixins.methods.simpleSuccessPopUp(vm)
              vm.users = vm.users.filter(el => el.id !== id)
            })
            .catch(() => {
              mixins.methods.errorPopUp(vm)
            })
        })
    },
    scrollEnded () {
      var sh = document.getElementById('inner').scrollHeight
      var st = document.getElementById('inner').scrollTop
      var oh = document.getElementById('inner').offsetHeight
      if (sh - st - oh + 1 <= 2) {
        if (this.isNextPage) {
          this.getNextPageUsers()
        }
      }
    }
  },
  async mounted () {
    await this.getObservedUsers()
    document.getElementById('inner').addEventListener('scroll', this.scrollEnded)
  }
}
</script>

<style scoped>
    img {
        width: 6vw;
        height: 6vw;
        object-fit: cover;
        border: 1px solid lightgray;
        border-radius: 5px;
    }
    #inner {
        overflow: hidden;
        overflow-y: auto;
        -webkit-transform: translate3d(0, 0, 0);
    }
    .userLink {
        cursor: pointer;
    }
</style>
