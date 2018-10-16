<template>
    <div v-if="isDataLoaded" >
        <div class="inner">
            <div class="list-group row">
                <div class="list-group-item list-group-item-action message my-2"
                     v-for="message in conversation"
                     :key="message.id"
                >
                    <p :class="{myMessage: message.receiverId == otherUserId}">
                        {{message.content}} </p>
                </div>
            </div>
        </div>
        <div>
            <textarea
            class = "form-control rounded-0 mt-3 mb-1"
            v-model="newMessage"
            rows="3"></textarea>
        </div>
        <div class="d-flex justify-content-end">
            <button
                class="btn btn-primary"
                @click="sendMessage">Wyślij</button>
        </div>
    </div>
</template>

<script>
import axios from 'axios'
export default {
  name: 'Conversation',
  data () {
    return {
      otherUserId: null,
      interval: null,
      conversation: [],
      isNextPage: true,
      isDataLoaded: false,
      nextPage: 2,
      newMessage: ''
    }
  },
  watch: {
    '$route.params.otherUserId': function (otherUserId) {
      this.refreshData()
    }
  },
  methods: {
    async refreshData () {
      this.otherUserId = this.$route.params.otherUserId
      let vm = this
      this.nextPage = 2
      this.isNextPage = true
      axios.get(`Messages?otherUserId=${this.otherUserId}&page=1`)
        .then(response => {
          vm.conversation = response.data
        })
    },
    sendMessage () {
      let vm = this
      axios.post('Messages', {receiverId: this.otherUserId, content: this.newMessage})
        .then(() => {
          vm.newMessage = ''
          vm.refreshData()
        })
      this.newMessage = ''
    }
  },
  async mounted () {
    await this.refreshData()
    this.isDataLoaded = true
    let vm = this
    this.interval = setInterval(() => {
      axios.get(`Messages/${vm.otherUserId}/isUpdate`)
        .then(response => {
          if (response.data === true) {
            console.log('time to update')
          } else {
            console.log('not updatin')
          }
        })
    }, 2000)
  },
  beforeDestroy () {
    clearInterval(this.interval)
  }
}
</script>

<style scoped>
    textarea {
        resize: none;
    }
    .message {
        margin-left: 5vh;
        margin-right: 5vh;
        width: auto !important;
        border: solid 1px lightgray;
        border-radius: 5px;
    }
    .myMessage {
        text-align: right;
    }
    .notMyMessage {
        text-align: left;
    }
    .inner {
        min-height: 200px;
        height: 61vh;
        max-height: 100%;
        overflow: hidden;
        overflow-y: auto;
        -webkit-transform: translate3d(0, 0, 0);
    }
</style>
