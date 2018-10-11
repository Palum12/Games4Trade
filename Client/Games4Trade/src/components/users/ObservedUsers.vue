<template>
    <div id="inner">
        <div class="list-group" v-for="user in users" :key="user.id">
            <router-link :to="'/user/'+user.id" tag="div">
                <div class="list-group-item list-group-item-action mb-1">
                    <div class="row">
                        <div class="col-3">
                            <img :src="`http://localhost:5000/api/users/${user.id}/photo`">
                        </div>
                        <div class="col-9 d-flex w-100 justify-content-between">
                            <h5 class="mb-1">{{user.login}}</h5>
                            <small>test co</small>
                        </div>
                        <p class="mb-1">{{user.description}}</p>
                        <small>tutaj coś mniejszego</small>
                    </div>
                </div>
                <button class="btn btn-danger mt-1 mb-2 mx-2">X</button>
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
      users: []
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
        width: 3vw;
        height: 3vw;
        object-fit: contain;
        border: 1px solid lightgray;
        border-radius: 5px;
    }
    #inner {
        overflow: hidden;
        overflow-y: auto;
        -webkit-transform: translate3d(0, 0, 0);
    }
</style>
