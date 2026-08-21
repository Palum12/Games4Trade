<template>
    <div>
        <div id="inner">
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
import { subscribeToMessages } from '../../services/messageHub'
export default {
  name: 'Conversation',
  data () {
    return {
      otherUserId: null,
      interval: null,
      unsubscribeFromMessages: null,
      conversation: [],
      isNextPage: true,
      isLoading: false,
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
      axios.get(`Messages?otherUserId=${this.otherUserId}&page=1`)
        .then(response => {
          vm.conversation = response.data
          vm.nextPage = 2
          vm.isNextPage = true
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
    },
    handleReceivedMessage (message) {
      if (message.senderId !== Number(this.otherUserId)) {
        return
      }

      if (!this.conversation.some(existingMessage => existingMessage.id === message.id)) {
        this.conversation.unshift(message)
      }

      axios.patch(`Messages?otherUserId=${this.otherUserId}`)
        .catch(error => console.log(error))
    },
    getNextPageMessages () {
      let vm = this
      vm.isLoading = true
      axios.get(`Messages?otherUserId=${this.otherUserId}&page=${this.nextPage}`)
        .then(response => {
          vm.conversation.push(...response.data)
          let data = response.data
          if (data.length === 0) {
            vm.isNextPage = false
          } else {
            vm.nextPage = vm.nextPage + 1
          }
          vm.isLoading = false
        })
    },
    scrollEnded () {
      var sh = document.getElementById('inner').scrollHeight
      var st = document.getElementById('inner').scrollTop
      var oh = document.getElementById('inner').offsetHeight
      if (sh - st - oh + 1 <= 2) {
        if (this.isNextPage && !this.isLoading) {
          this.getNextPageMessages()
        }
      }
    },
    addInterval () {
      let vm = this
      this.interval = setInterval(() => {
        axios.get(`Messages/${vm.otherUserId}/isUpdate`)
          .then(response => {
            if (response.data === true) {
              var latestId = vm.conversation[0]?.id ?? 0
              let messages
              axios.get(`Messages?otherUserId=${this.otherUserId}&page=1`)
                .then(response => {
                  messages = response.data
                  messages = messages.filter(el => el.id > latestId)
                  vm.conversation.unshift(...messages)
                  while (vm.conversation.length / 20 > vm.nextPage - 1) {
                    vm.nextPage = vm.nextPage + 1
                  }
                })
              axios.patch(`Messages?otherUserId=${vm.otherUserId}`)
            }
          })
      }, 2000)
    }
  },
  async mounted () {
    this.unsubscribeFromMessages = subscribeToMessages(this.handleReceivedMessage)
    await this.refreshData()
    document.getElementById('inner').addEventListener('scroll', this.scrollEnded)
    this.addInterval()
  },
  beforeUnmount () {
    this.unsubscribeFromMessages?.()
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
    #inner {
        min-height: 200px;
        height: 61vh;
        max-height: 100%;
        overflow: hidden;
        overflow-y: auto;
        -webkit-transform: translate3d(0, 0, 0);
    }
</style>
