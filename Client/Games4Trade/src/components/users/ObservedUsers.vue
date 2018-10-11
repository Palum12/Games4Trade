<template>
    <div id="inner">
        <div class="list-group" v-for="user in users" :key="user.id">
            <router-link :to="'/user/'+user.id" tag="div">
                <div class="list-group-item list-group-item-action mb-2">
                    <div class="row">
                        <div class="col-1">
                            <div class="row">
                                <p class="mb-1 font-weight-bold">{{user.login}}</p>
                            </div>
                            <div class="row">
                                <img :src="`http://localhost:5000/api/users/${user.id}/photo`">
                            </div>
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
                            <button class="btn btn-danger mt-1 mb-2 mx-2">Przestań<br>obserwować</button>
                        </div>
                    </div>
                </div>
            </router-link>
        </div>
    </div>
</template>

<script>
import axios from 'axios'
export default {
  name: 'ObservedUsers',
  props: {
    userId: Number
  },
  data () {
    return {
      users: [],
      noDescriptionMessage: 'Wygląda na to, że ten użytkonik nie posiada jeszcze opisu!',
      noGenresMessage: 'Ten użytkonik nie polubił żadnych gatunków!',
      noSystemsMessage: 'Tego użytkownika nie interesują żadne systemy!'
    }
  },
  methods: {
    async getObservedUsers () {
      let vm = this
      await axios.get(`users/${this.userId}/observed`)
        .then(response => {
          vm.users = response.data
        })
        .catch(error => {
          console.log(error)
        })
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
    scrollEnded () {
      var sh = document.getElementById('inner').scrollHeight
      var st = document.getElementById('inner').scrollTop
      var oh = document.getElementById('inner').offsetHeight
      if (sh - st - oh + 1 < 2) {
        // tutaj odśwież stronę
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
</style>
