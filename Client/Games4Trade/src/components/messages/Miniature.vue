<template>
    <div class="row">
        <div class="col-3">
            <img :src="getPhotoUrl(message.otherUserId)">
        </div>
        <div class="col-9">
            <div class="row">
                <div class="col-6">
                    <p style="font-weight: bold">
                        {{message.otherUser.login}}
                    </p>
                </div>
                <div class="col-6">
                    <small>{{message.dateCreated.substring(2,10)}} </small>
                    <small>{{message.dateCreated.substring(11,16)}}</small>
                </div>
            </div>
            <div class="row">
                <p :class="{newMessage: !message.isDelivered && message.otherUserId === userId}">
                    {{shortenString(message.content)}}</p>
            </div>
        </div>
    </div>
</template>

<script>
import axios from 'axios'

export default {
  name: 'Miniature',
  props: ['message', 'userId'],
  methods: {
    getPhotoUrl (userId) {
      const baseUrl = axios.defaults.baseURL ? axios.defaults.baseURL.replace(/\/$/, '') : ''
      return `${baseUrl}/users/${userId}/photo`
    },
    shortenString (text) {
      if (text.length > 60) {
        return text.substring(0, 57) + '...'
      }
      return text
    }
  }
}
</script>

<style scoped>
    img {
        width: 4vw;
        height: 4vw;
        object-fit: cover;
    }
    .newMessage {
        font-weight: bold;
    }
</style>
