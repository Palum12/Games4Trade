<template>
    <div class="row ml-5 no-gutters ">
        <div class="col-3">
            <div class="row">
                <h5>Najnowsze wiadmości</h5>
            </div>
            <div class="list-group row newest miniatures" v-if="dataLoaded" >
                <div class="list-group-item list-group-item-action"
                     v-for="message in conversations"
                     :key="message.receiverId"
                     @click="readMessage(message)">
                        <Miniature
                                :message="message"
                                :user-id="userId"></Miniature>
                </div>
            </div>
        </div>
        <div class="col-8 ml-5">
            <div class="row">
                <h5>Rozmowa</h5>
            </div>
            <router-view class="conversation"></router-view>
        </div>
    </div>
</template>

<script>
import axios from 'axios'
import Miniature from '../components/messages/Miniature'
export default {
  name: 'Messages',
  components: {
    Miniature
  },
  data () {
    return {
      dataLoaded: false,
      conversations: [],
      hasUnSavedChanges: false,
      userId: Number
    }
  },
  methods: {
    async getNewestMessages () {
      let vm = this
      await axios.get('messages')
        .then(response => {
          vm.conversations = response.data
          vm.dataLoaded = true
        })
        .catch(error => {
          console.log(error)
        })
    },
    readMessage (message) {
      message.isDelivered = true
      axios.patch(`Messages?otherUserId=${message.receiverId}`)
      this.$router.push(`/messages/${message.receiverId}/conversation`)
    }
  },
  async mounted () {
    let vm = this
    await this.$store.dispatch('getUserId')
      .then(response => {
        vm.userId = response.data
      })
    await this.getNewestMessages()
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
    .newest {
        border: solid 1px black;
        border-radius: 5px;
    }
    .miniatures {
        min-height: 200px;
        height: 78vh;
        max-height: 100%;
        overflow: hidden;
        overflow-y: auto;
        -webkit-transform: translate3d(0, 0, 0);
    }
    .conversation{
        min-height: 200px;
        height: 78vh;
        max-height: 100%;
        border: solid 1px darkgrey;
        border-radius: 5px;
    }
</style>
