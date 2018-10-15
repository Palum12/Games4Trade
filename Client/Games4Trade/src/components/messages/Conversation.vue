<template>
    <div class="inner">
        <div >
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
            <textarea></textarea>
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
      nextPage: 2
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
    }
  },
  async mounted () {
    await this.refreshData()
  }
}
</script>

<style scoped>
    .inner {
        overflow: hidden;
        overflow-y: auto;
        -webkit-transform: translate3d(0, 0, 0);
    }
</style>
