<template>
    <div >
        <div class="inner">
            <div class="list-group row">
                <div class="list-group-item list-group-item-action"
                     v-for="message in conversation"
                     :key="message.id"
                >
                    <p>{{message.content}} </p>
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
      conversation: [],
      isNextPage: true,
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
      let id = this.$route.params.otherUserId
      let vm = this
      this.nextPage = 2
      this.isNextPage = true
      axios.get(`Messages?otherUserId=${id}&page=1`)
        .then(response => {
          vm.conversation = response.data
        })
    },
    sendMessage () {
      console.log(this.newMessage)
      this.newMessage = ''
    }
  },
  async mounted () {
    await this.refreshData()
  }
}
</script>

<style scoped>
    textarea {
        resize: none;
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
